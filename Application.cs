using System;
using BotShop.Models;
using BotShop.Utils;
using BotShopCore.Utils;

namespace BotShop {
  public class Application {
    private readonly BotManager _botManager = new BotManager();

    public void Start() {
      DotEnvUtils.InjectDotEnvVars();
      _botManager.UpdateBots();
      TimeUtils.SetInterval(TimeSpan.FromSeconds(30), () => _botManager.UpdateBots());
    }
  }
}
