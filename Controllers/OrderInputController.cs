using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace ImportShopBot.Controllers {
  public class OrderInputController {
    private Message Message { get; }

    public OrderInputController(Message message) => Message = message;

    [UsedImplicitly]
    [MessageHandler(Queries.AnySequence, Queries.InputAddress + Queries.FullMatch)]
    public string AcceptAddress() => Queries.InputAddress + Queries.Separator + Message.Text;

    [UsedImplicitly]
    [MessageHandler(Queries.AnySequence, Queries.AcceptAddress + Queries.FullMatch)]
    public string AcceptPhone() => Queries.InputPhone + Queries.Separator + Message.Text;
  }
}