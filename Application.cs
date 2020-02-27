using System;
using ImportShopBot.Models;
using ImportShopBot.Utils;

namespace ImportShopBot {
  public class Application {
    private readonly BotManager _botManager = new BotManager();

    public void Start() {
      _botManager.UpdateBots();
      TimeUtils.SetInterval(TimeSpan.FromSeconds(30), () => _botManager.UpdateBots());
    }
  }
}