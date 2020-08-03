using BotShop.Telegram;
using BotShopCore.Models.Entities;

namespace BotShop.Models {
  public class BotHub {
    public Account Account { get; }

    private readonly TelegramBot _telegramBot;

    public BotHub(Account account) {
      Account = account;
      _telegramBot = new TelegramBot(account);
    }

    public void Start() {
      _telegramBot.Start(typeof(BotHub).Assembly);
    }

    public void Stop() {
      _telegramBot.Stop();
    }
  }
}
