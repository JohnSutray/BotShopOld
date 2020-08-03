using BotCore.Interfaces;
using BotCore.Interfaces.BotEntities;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BotShop.Telegram {
  public class TelegramEventSource : IBotEventSource {
    private class TelegramInputInputQuery : IBotInputQuery {
      public int ChatId { get; }

      public string Payload { get; }

      public TelegramInputInputQuery(CallbackQuery query) {
        ChatId = query.From.Id;
        Payload = query.Data;
      }
    }

    public TelegramEventSource(ITelegramBotClient client) {
      client.OnMessage += MessageHandler;
      client.OnCallbackQuery += QueryHandler;
    }

    public event MessageHandler OnMessage;
    public event QueryHandler OnQuery;

    private void MessageHandler(object sender, MessageEventArgs args)
      => OnMessage?.Invoke(
        new TelegramInputMessage(args.Message),
        new TelegramInputChat(args.Message.From)
      );

    private void QueryHandler(object sender, CallbackQueryEventArgs args)
      => OnQuery?.Invoke(
        new TelegramInputInputQuery(args.CallbackQuery),
        new TelegramInputChat(args.CallbackQuery.From)
      );
  }
}
