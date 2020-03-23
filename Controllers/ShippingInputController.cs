using Flurl;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace ImportShopBot.Controllers {
  public class ShippingInputController {
    private Message Message { get; }

    public ShippingInputController(Message message) => Message = message;

    [UsedImplicitly]
    [MessageHandler(Queries.AnySequence, Queries.InputShippingAddress)]
    public string GetShippingAddress() {
      return Url.Combine(Queries.InputShippingAddress, Message.Text);
    }
  }
}