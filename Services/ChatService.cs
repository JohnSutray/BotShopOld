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

    public ChatService(ApplicationContext context, Account account, User user)
      : base(context, c => c.Chats) {
      Account = account;
      User = user;
    }

    public void UpdateChatQuery(string query) => UpdateById(User.Id, chat => chat.Query = query);

    public void UpdateChatAddress(string address) => UpdateById(User.Id, chat => chat.Address = address);

    public void UpdateChatPhone(string phone) => UpdateById(
      User.Id,
      chat => chat.Phone = phone
    );

    public Chat GetCurrentChat() => ById(User.Id);

    public Chat EnsureChatSaved() {
      return ById(User.Id) is {} existingChat
        ? existingChat
        : AddEntity(new Chat {
          Id = User.Id,
          AccountId = Account.Id,
          Query = "menu",
          Address = "",
          Phone = "",
          FirstName = User.FirstName ?? User.Username,
          LastName = User.LastName ?? "Не указано"
        });
    }
  }
}