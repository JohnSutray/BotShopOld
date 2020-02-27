using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportShopBot.Extensions;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Account;
using ImportShopCore.Models.Telegram;
using Telegram.Bot.Types;
using Chat = ImportShopCore.Models.Telegram.Chat;

namespace ImportShopBot.Services {
  [Service]
  public class ChatService : RepositoryService<Chat> {
    private Account Account { get; }
    private User User { get; }

    public ChatService(
      ApplicationContext context,
      Account account,
      User user
    ) : base(context, c => c.Chats) {
      Account = account;
      User = user;
    }

    public async Task EnsureChatSaved() {
      if (await ByIdAsync(User.Id) != null) {
        return;
      }

      await AddEntityAsync(new Chat {
        Id = User.Id,
        AccountId = Account.Id
      });
    }
  }
}