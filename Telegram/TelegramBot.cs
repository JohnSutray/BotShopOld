using System.Reflection;
using BotCore;
using BotShop.Models;
using BotShop.Services;
using BotShop.Services.I18N;
using BotShopCore;
using BotShopCore.Extensions;
using BotShopCore.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BotShop.Telegram {
  public class TelegramBot : Bot {
    private readonly ITelegramBotClient _client;
    private readonly Account _account;

    public TelegramBot(Account account) {
      _client = new TelegramBotClient(account.TelegramToken);
      _account = account;
      EventSource = new TelegramEventSource(_client);
    }

    protected override void ConfigureServices(IServiceCollection services) => services
      .AddAssemblyServices(typeof(TelegramBot).Assembly)
      .AddI18NService(new I18NOptions {
        Languages = new []{ "ru" },
        BasePath = "I18n",
        DefaultLanguage = "ru"
      })
      .AddSingleton(_client)
      .AddSingleton(_account)
      .AddTransient<ApplicationContext>();


    public override void Start(Assembly workingAssembly) {
      base.Start(workingAssembly);
      _client.StartReceiving();
    }

    public override void Stop() {
      base.Stop();
      _client.StopReceiving();
    }
  }
}
