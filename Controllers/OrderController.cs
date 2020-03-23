using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using ImportShopBot.Extensions;
using ImportShopBot.Extensions.Markup;
using ImportShopBot.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace ImportShopBot.Controllers {
  public class OrderController {
    private RouteValueDictionary RouteValues { get; }
    private ReplyService ReplyService { get; }
    private CartService CartService { get; }
    private ChatService ChatService { get; }

    public OrderController(
      ReplyService replyService,
      CartService cartService,
      RouteValueDictionary routeValues,
      ChatService chatService
    ) {
      ReplyService = replyService;
      CartService = cartService;
      RouteValues = routeValues;
      ChatService = chatService;
    }

    [UsedImplicitly]
    [QueryHandler(Queries.Order)]
    public async Task StartOrderProcessing() {
      if (await CartService.IsCartEmpty()) {
        await SendCannotCompleteEmptyOrder();
        return;
      }

      await SendShippingConfirmation();
    }

    [UsedImplicitly]
    [QueryHandler(Queries.InputShippingAddress)]
    public async Task InputShippingAddress() {
      var chat = await ChatService.GetCurrentChat();

      await SendInputAddress(chat.Address);
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.InputShippingAddress +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Address +
      Queries.EndInterpolation
    )]
    public async Task SaveShippingAddress() {
      var address = RouteValues.GetValue(Variables.Address);
      await ChatService.UpdateChatAddress(address);
      var chat = await ChatService.GetCurrentChat();

      await SendInputPhoneMessage(chat.Phone);
    }

    private async Task SendInputAddress(string previousAddress) => await ReplyService.SendTextAsync(
      Labels.InputAddress,
      previousAddress.ToKeyboardButton().ToKeyboard()
    );

    private async Task SendInputPhoneMessage(string previousPhone) => await ReplyService.SendTextAsync(
      Labels.InputPhone,
      previousPhone.ToKeyboardButton().ToKeyboard()
    );

    private async Task SendCannotCompleteEmptyOrder() => await ReplyService.SendTextAsync(
      Labels.CannotCompleteEmptyOrder,
      Markups.ToMainMenuButton.ToInlineKeyboard()
    );

    private async Task SendShippingConfirmation() => await ReplyService.SendTextAsync(
      Labels.AreYouNeedShipping,
      Markups.ShippingConfirmationKeyboard
    );
  }
}