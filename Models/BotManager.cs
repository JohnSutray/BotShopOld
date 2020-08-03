using System.Collections.Generic;
using System.Linq;
using BotShopCore;
using BotShopCore.Models.Entities;

namespace BotShop.Models {
  public class BotManager {
    private readonly List<BotHub> _hubs = new List<BotHub>();
    private readonly ApplicationContext _applicationContext;

    public BotManager() => _applicationContext = new ApplicationContext();

    private List<Account> GetCurrentAccounts() => _applicationContext.Accounts.ToList();

    public void UpdateBots() {
      var currentAccounts = GetCurrentAccounts();
      var activeAccounts = _hubs.Select(
        bot => currentAccounts.First(account => account.Id == bot.Account.Id)
      ).ToList();
      var accountsToBootstrap = currentAccounts.Except(activeAccounts).ToList();
      var accountsToKill = activeAccounts.Except(currentAccounts).ToList();

      accountsToBootstrap.ForEach(StartHub);
      accountsToKill.ForEach(StopHub);
    }

    private void StartHub(Account account) {
      var bot = new BotHub(account);
      bot.Start();
      _hubs.Add(bot);
    }

    private void StopHub(Account account) => _hubs.First(b => b.Account.Id == account.Id).Stop();
  }
}
