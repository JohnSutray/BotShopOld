using BotCore.Interfaces;
using BotCore.Interfaces.BotEntities;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotShop.Services {
  [Service(ResolveType = typeof(IBotChatService))]
  [Service(ResolveType = typeof(ChatService))]
  public class ChatService : RepositoryService<Chat>, IBotChatService {
    private readonly Account _account;

    public ChatService(ApplicationContext context, Account account) : base(context) => _account = account;

    public void UpdateLastExecutedQuery(IBotInputChat inputChat, string query) =>
      UpdateById(inputChat.Id, chat => chat.LastExecutedQuery = query);

    public void UpdateChatAddress(IBotInputChat botInput, string address) =>
      UpdateById(botInput.Id, chat => chat.Address = address);

    public void UpdateChatPhone(IBotInputChat inputChat, string phone) =>
      UpdateById(inputChat.Id, chat => chat.Phone = phone);

    public void EnsureChatSaved(IBotInputChat inputChat) {
      if (FindChat(inputChat) != null) return;

      AddEntity(new Chat {
        Id = inputChat.Id,
        AccountId = _account.Id,
        LastExecutedQuery = "menu",
        Address = "",
        Phone = "",
        FirstName = inputChat.FirstName,
        LastName = inputChat.LastName ?? "Не указано",
        PlatformId = inputChat.PlatformId
      });
    }

    public IBotChat FindChat(IBotInputChat inputChat) =>
      ByPattern(chat => chat.Id == inputChat.Id && chat.PlatformId == inputChat.PlatformId);

    protected override DbSet<Chat> Set => Context.Chats;
  }
}
