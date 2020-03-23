using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ImportShopBot.Extensions;
using ImportShopBot.Extensions.Query;
using ImportShopBot.Services;
using ImportShopCore;
using ImportShopCore.Extensions;
using ImportShopCore.Models.Entities;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Chat = ImportShopCore.Models.Entities.Chat;

namespace ImportShopBot.Models {
  public class Bot {
    public Bot(Account account, IConfiguration configuration) {
      Account = account;
      Configuration = configuration;
      _queryActions.ForEach(ValidateQueryActions);
      _messageActions.ForEach(ValidateMessageActions);
    }

    public Account Account { get; }
    private TelegramBotClient BotClient { get; set; }
    private IConfiguration Configuration { get; }

    private readonly List<ControllerAction<string>> _queryActions = typeof(Bot)
      .Assembly
      .GetTypes()
      .Where(controller => controller.IsQueryController())
      .SelectMany(controller => controller.GetQueryActions())
      .ToList();

    private readonly List<ControllerAction<MessageActionRoutingData>> _messageActions = typeof(Bot)
      .Assembly
      .GetTypes()
      .Where(controller => controller.IsMessageController())
      .SelectMany(controller => controller.GetMessageActions())
      .ToList();
    
    
    private void ValidateQueryActions(ControllerAction<string> action) {
      if (action.Handler.ReturnType != typeof(Task)) {
        throw new ArgumentOutOfRangeException(
          "\n" +
          "Invalid controller action return type.\n" +
          $"${action.Controller.Name}.${action.Handler.Name}() => ${action.Handler.ReturnType.Name}\n" +
          $"Return data must be of type ${typeof(Task).Name}" +
          "\n"
        );
      }
    }

    private void ValidateMessageActions(ControllerAction<MessageActionRoutingData> action) {
      if (action.Handler.ReturnType != typeof(string)) {
        throw new ArgumentOutOfRangeException(
          "\n" +
          "Invalid controller action return type.\n" +
          $"${action.Controller.Name}.${action.Handler.Name}() => ${action.Handler.ReturnType.Name}\n" +
          $"Return data must be of type ${typeof(string).Name}" +
          "\n"
        );
      }
    }

    private void HandleMessage(object sender, MessageEventArgs args) =>
      Task.Run(() => RootMessageHandler(args.Message));

    private void HandleQuery(object sender, CallbackQueryEventArgs args) {
      Task.Run(() => RootQueryHandler(args.CallbackQuery));
    }

    public void Start() {
      BotClient = new TelegramBotClient(Account.TelegramToken) { MessageOffset = 1000 };
      BotClient.OnMessage += HandleMessage;
      BotClient.OnCallbackQuery += HandleQuery;
      BotClient.StartReceiving();
    }

    public void Stop() => BotClient.StopReceiving();

    private bool IsMessageActionMatch(
      ControllerAction<MessageActionRoutingData> action,
      Chat chat,
      Message message
    ) => action.RoutingData.MessagePattern.IsMatch(message.Text)
         && action.RoutingData.QueryPattern.IsMatch(chat.Query);

    private void LogNoMessageHandler(Message message, Chat chat) => Console.WriteLine(
      $"There is no handler for message {message.Text}\n" +
      $"Last visited query:             {chat.Query}" +
      "\n\n"
    );

    private void LogEventSource(User user) => Console.WriteLine(
      $"From: {user.Id} - {user.FirstName}"
    );

    private void LogQueryMatch(ControllerAction<string> action, RouteValueDictionary matchedData) {
      Console.WriteLine($"Handler: {action.RoutingData}");
      Console.WriteLine($"Matched: {(matchedData.Any() ? "" : "none")}");
      matchedData.Select(pair => $"\t{pair.Key}: {pair.Value}").ToList().ForEach(Console.WriteLine);
      Console.WriteLine("\n");
    }

    private async Task RootMessageHandler(Message message) {
      Console.WriteLine($"Incoming message: {message.Text}");
      LogEventSource(message.From);

      var serviceCollection = CreateMessageHandlerServiceProvider(message);
      await using var provider = serviceCollection.BuildServiceProvider();
      using var scope = provider.CreateScope();
      var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();

      var chat = await chatService.EnsureChatSaved();
      await scope.ServiceProvider.GetRequiredService<MessageService>()
        .SaveMessageAsync(message);

      var messageAction = _messageActions.FirstOrDefault(
        action => IsMessageActionMatch(action, chat, message)
      );

      if (messageAction == null) {
        LogNoMessageHandler(message, chat);
        return;
      }

      var controller = scope.ServiceProvider.GetRequiredService(messageAction.Controller);
      var queryMap = messageAction.Handler.Invoke(controller, null) as string;
      var query = new CallbackQuery { Data = queryMap, From = message.From };
      
      await RootQueryHandler(query);
    }

    private async Task RootQueryHandler(CallbackQuery query) {
      LogEventSource(query.From);
      Console.WriteLine($"Query:   {query.Data}");

      var queryAction = _queryActions.FirstOrDefault(action => action.RoutingData.IsRouteMatched(query.Data));
      
      if (queryAction == null) {
        Console.WriteLine($"There is no handler for query ${query.Data}\n\n");
        return;
      }

      var matchedData = queryAction.RoutingData.MatchRoute(query.Data);

      LogQueryMatch(queryAction, matchedData);

      var serviceCollection = CreateQueryHandlerServiceProvider(query, matchedData);
      await using var provider = serviceCollection.BuildServiceProvider();
      using var scope = provider.CreateScope();

      if (queryAction.ClearDisplayBeforeHandle) {
        var messageService = scope.ServiceProvider.GetRequiredService<MessageService>();
        await messageService.ClearMessagesAsync();
      }

      await HandleQuery(scope, queryAction.Controller, queryAction.Handler);

      var chatService = scope.ServiceProvider.GetRequiredService<ChatService>();
      await chatService.UpdateChatQuery(query.Data);
      
      
    }

    private async Task HandleQuery(IServiceScope scope, Type controller, MethodInfo handler) {
      var controllerInstance = scope.ServiceProvider.GetRequiredService(controller);
      var handleTask = handler.Invoke(controllerInstance, null) as Task;

      try {
        await handleTask;
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }
    }

    private IServiceCollection CreateQueryHandlerServiceProvider(
      CallbackQuery query,
      RouteValueDictionary matchedData
    ) => Services
      .AddSingleton(query)
      .AddSingleton(matchedData)
      .AddSingleton(query.From);

    private IServiceCollection CreateMessageHandlerServiceProvider(Message message) => Services
      .AddSingleton(message)
      .AddSingleton(message.From);

    private IServiceCollection Services {
      get {
        var services = new ServiceCollection();

        services.AddSingleton(BotClient)
          .AddSingleton(Account)
          .AddSingleton(Configuration)
          .AddTransient<ApplicationContext>()
          .AddAssemblyServices(typeof(Bot).Assembly);

        _messageActions.ForEach(action => services.AddTransient(action.Controller));
        _queryActions.ForEach(action => services.AddTransient(action.Controller));

        return services;
      }
    }
  }
}