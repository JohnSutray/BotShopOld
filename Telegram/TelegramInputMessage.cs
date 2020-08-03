using BotCore.Interfaces;
using Telegram.Bot.Types;

namespace BotShop.Telegram {
  public class TelegramInputMessage : IBotInputMessage {
    public int Id { get; }

    public int ChatId { get; }

    public string Content { get; }

    public TelegramInputMessage(Message message) {
      Id = message.MessageId;
      ChatId = message.From.Id;
      Content = message.Text;
    }
  }
}
