using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;
using Telegram.Bot.Types;
using Chat = ImportShopCore.Models.Entities.Chat;

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

    public async Task UpdateChatQuery(string query) => await UpdateByIdAsync(
      User.Id,
      chat => chat.Query = query
    );

    public async Task UpdateChatAddress(string address) => await UpdateByIdAsync(
      User.Id,
      chat => chat.Address = address
    );

    public async Task<Chat> GetCurrentChat() => await ByIdAsync(User.Id);

    public async Task<Chat> EnsureChatSaved() {
      if (await ByIdAsync(User.Id) is {} existingChat) {
        return existingChat;
      }

      return await AddEntityAsync(new Chat {
        Id = User.Id,
        AccountId = Account.Id
      });
    }
  }
}