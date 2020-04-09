using System;
using System.Linq;
using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ImportShopBot.Services {
  [Service]
  public class MessageService : RepositoryService<TelegramMessage> {
    private TelegramBotClient Client { get; }
    private User User { get; }

    public MessageService(
      ApplicationContext context,
      User user,
      TelegramBotClient client
    ) : base(
      context,
      applicationContext => applicationContext.Messages
    ) {
      User = user;
      Client = client;
    }

    public void ClearMessages() {
      var messages = RemoveManyByPattern(message => message.ChatId == User.Id);
      Task DeleteMessage(TelegramMessage message) => Client.DeleteMessageAsync(User.Id, message.Id);

      try {
        Task.WhenAll(messages.Select(DeleteMessage)).GetAwaiter().GetResult();
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }
    }

    public void SaveMessage(Message message) => AddEntity(
      new TelegramMessage {
        Id = message.MessageId,
        ChatId = User.Id
      }
    );
  }
}