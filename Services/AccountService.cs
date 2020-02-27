using System.Linq;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models.Account;

namespace ImportShopBot.Services {
  [Service]
  public class AccountService {
    public IQueryable<Account> Accounts => ApplicationContext.Accounts;
    private ApplicationContext ApplicationContext { get; }

    public AccountService(ApplicationContext applicationContext)
      => ApplicationContext = applicationContext;
  }
}