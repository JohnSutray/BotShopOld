using System.Linq;
using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Telegram;
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

    public async Task ClearMessagesAsync() {
      var messages = await RemoveManyByPatternAsync(message => message.ChatId == User.Id);
      Task DeleteMessage(TelegramMessage message) => Client.DeleteMessageAsync(User.Id, message.Id);

      await Task.WhenAll(messages.Select(DeleteMessage));
    }

    public async Task SaveMessageAsync(Message message) => await AddEntityAsync(
      new TelegramMessage {
        Id = message.MessageId,
        ChatId = User.Id
      }
    );
  }
}