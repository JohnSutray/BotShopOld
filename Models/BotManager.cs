using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Models.Account;
using ImportShopCore.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ImportShopBot.Models {
  public class BotManager {
    public BotManager() {
      _applicationContext = new ApplicationContext(_configuration);
    }

    private readonly List<Bot> _tmBotInstances = new List<Bot>();

    private readonly IConfiguration _configuration = ConfigurationUtils.CreateAppSettingsConfiguration();

    private readonly ApplicationContext _applicationContext;

    private Task<List<Account>> Accounts => _applicationContext.Accounts.ToListAsync();

    public async void UpdateBots() {
      var allAccounts = await Accounts;
      
      var activeAccounts = _tmBotInstances.Select(
        bot => allAccounts.First(account => account.Id == bot.Account.Id)
      );
      var accountsToBootstrap = allAccounts.Except(activeAccounts);
      var accountsToKill = activeAccounts.Except(allAccounts);

      accountsToBootstrap.ToList().ForEach(BootstrapBot);
      accountsToKill.ToList().ForEach(KillBot);
    }

    private void BootstrapBot(Account account) {
      var bot = new Bot(account, _configuration);
      bot.Start();
      _tmBotInstances.Add(bot);
    }

    private void KillBot(Account account) => _tmBotInstances
      .First(b => b.Account.Id == account.Id)
      .Stop();
  }
}