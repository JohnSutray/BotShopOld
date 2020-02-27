using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Extensions;
using ImportShopBot.Extensions.Query;
using ImportShopBot.Services;
using ImportShopCore;
using ImportShopCore.Extensions;
using ImportShopCore.Models.Account;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ImportShopBot.Models {
  public class Bot {
    public Bot(Account account, IConfiguration configuration) {
      Account = account;
      Configuration = configuration;
    }

    public Account Account { get; }
    private TelegramBotClient BotClient { get; set; }
    private IConfiguration Configuration { get; }

    private readonly List<ControllerAction<string>> _queryActions = typeof(Bot)
      .Assembly
      .GetTypes()
      .Where(IsQueryController)
      .SelectMany(controller => controller.GetQueryHandlers())
      .ToList();

    private readonly List<ControllerAction<Regex>> _messageHandlers = typeof(Bot)
      .Assembly
      .GetTypes()
      .Where(IsMessageController)
      .SelectMany(controller => controller.GetMessageHandlers())
      .ToList();

    private async void HandleMessage(object sender, MessageEventArgs args) => await RootMessageHandler(args.Message);

    private async void HandleQuery(object sender, CallbackQueryEventArgs args) =>
      await RootQueryHandler(args.CallbackQuery);

    private static bool IsQueryController(Type controller) => controller.HasMethodsWithAttribute<QueryHandler>();
    private static bool IsMessageController(Type controller) => controller.HasMethodsWithAttribute<MessageHandler>();

    public void Start() {
      BotClient = new TelegramBotClient(Account.TelegramToken);
      BotClient.OnMessage += HandleMessage;
      BotClient.OnCallbackQuery += HandleQuery;
      BotClient.StartReceiving();
    }

    public void Stop() => BotClient.StopReceiving();

    private async Task RootMessageHandler(Message message) {
      foreach (var messageHandler in _messageHandlers) {
        if (!messageHandler.HandleBy.IsMatch(message.Text)) continue;

        var serviceCollection = CreateMessageHandlerServiceProvider(message, messageHandler.Controller);
        await using var provider = serviceCollection.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService(messageHandler.Controller);
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
        var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();

        await chatService.EnsureChatSaved();
        await messageService.SaveMessageAsync(message);

        if (messageHandler.ClearDisplayBeforeHandle) {
          await messageService.ClearMessagesAsync();
        }

        var result = messageHandler.Handler.Invoke(controller, null) as string;
        var query = new CallbackQuery {
          Data = result,
          From = message.From
        };

        await RootQueryHandler(query);
        
        return;
      }
    }

    private async Task RootQueryHandler(CallbackQuery query) {
      _queryActions.ForEach(a=> Console.WriteLine(a.HandleBy));
      
      foreach (var queryAction in _queryActions) {
        var matchedData = queryAction.HandleBy.MatchRoute(query.Data);

        if (matchedData == null) continue;

        var serviceCollection = CreateQueryHandlerServiceProvider(query, matchedData, queryAction.Controller);
        await using var provider = serviceCollection.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService(queryAction.Controller);

        if (queryAction.ClearDisplayBeforeHandle) {
          var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
          await messageService.ClearMessagesAsync();
        }

        var result = queryAction.Handler.Invoke(controller, null) as Task;

        await result;
        
        return;
      }

      Console.WriteLine($"There is no handler for query ${query.Data}");
    }

    private IServiceCollection CreateQueryHandlerServiceProvider(
      CallbackQuery query,
      RouteValueDictionary matchedData,
      Type controller
    ) => Services
      .AddSingleton(query)
      .AddSingleton(matchedData)
      .AddSingleton(query.From)
      .AddTransient(controller);

    private IServiceCollection CreateMessageHandlerServiceProvider(Message message, Type controller) => Services
      .AddSingleton(message)
      .AddSingleton(message.From)
      .AddTransient(controller);

    private IServiceCollection Services => new ServiceCollection()
      .AddSingleton(BotClient)
      .AddSingleton(Account)
      .AddSingleton(Configuration)
      .AddTransient<ApplicationContext>()
      .AddAllAssemblyServices(typeof(Bot).Assembly);
  }
}