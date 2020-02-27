using ImportShopCore.Models.Telegram;
using Telegram.Bot.Types;

namespace ImportShopBot.Extensions {
  public static class MessageExtensions {
    public static TelegramMessage ToTmMessageId(this Message message) => new TelegramMessage {
      Id = message.MessageId
    };
  }
}