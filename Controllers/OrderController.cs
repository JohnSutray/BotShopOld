using System;
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
    private OrderService OrderService { get; }

    public OrderController(
      ReplyService replyService,
      CartService cartService,
      RouteValueDictionary routeValues,
      ChatService chatService,
      OrderService orderService
    ) {
      ReplyService = replyService;
      CartService = cartService;
      RouteValues = routeValues;
      ChatService = chatService;
      OrderService = orderService;
    }

    [UsedImplicitly]
    [QueryHandler(Queries.Order)]
    public void StartOrderProcessing() {
      if (CartService.IsCartEmpty()) {
        SendCannotCompleteEmptyOrderPage();
        return;
      }

      SendShippingConfirmationPage();
    }

    [UsedImplicitly]
    [QueryHandler(Queries.InputAddress)]
    public void InputAddress() {
      var chat = ChatService.GetCurrentChat();

      SendInputAddressPage(chat.Address);
    }

    [UsedImplicitly]
    [QueryHandler(Queries.AcceptAddress)]
    public void AcceptShippingAddress() {
      var address = RouteValues.GetValue(Queries.Address);
      ChatService.UpdateChatAddress(address);
      var chat = ChatService.GetCurrentChat();

      SendInputPhonePage(chat.Phone);
    }

    [UsedImplicitly]
    [QueryHandler(Queries.AcceptPhone)]
    public void AcceptPhone() {
      var phone = RouteValues.GetValue(Queries.Phone);
      ChatService.UpdateChatPhone(phone);
      OrderService.SaveOrder();
      CartService.ClearCart();

      SendOrderCompletedPage();
    }

    private void SendInputAddressPage(string previousAddress) => ReplyService.SendText(
      Labels.InputAddress,
      previousAddress.ToKeyboardButton().ToKeyboard()
    );

    private void SendInputPhonePage(string previousPhone) => ReplyService.SendText(
      Labels.InputPhone,
      previousPhone.ToKeyboardButton().ToKeyboard()
    );

    private void SendCannotCompleteEmptyOrderPage() => ReplyService.SendText(
      Labels.CannotCompleteEmptyOrder,
      Markups.ToMainMenuButton.ToInlineKeyboard()
    );

    private void SendShippingConfirmationPage() => ReplyService.SendText(
      Labels.AreYouNeedShipping,
      Markups.ShippingConfirmationKeyboard
    );

    private void SendOrderCompletedPage() => ReplyService.SendText(
      Labels.OrderCompleted,
      Markups.ToMainMenuButton.ToInlineKeyboard()
    );
  }
}