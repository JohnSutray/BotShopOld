using BotCore.Attributes;
using BotCore.Interfaces;
using BotShop.Extensions;
using BotShop.Models.ViewModels;
using BotShop.Services;
using BotShop.Services.I18N;
using Microsoft.AspNetCore.Routing;

namespace BotShop.Controllers {
  public class OrderController {
    private readonly RouteValueDictionary _routeValues;
    private readonly CartService _cartService;
    private readonly ChatService _chatService;
    private readonly OrderService _orderService;
    private readonly ViewModelService _viewModelService;
    private readonly II18NService _i18NService;
    private readonly IBotInputChat _chat;

    public OrderController(
      CartService cartService,
      RouteValueDictionary routeValues,
      ChatService chatService,
      OrderService orderService,
      ViewModelService viewModelService,
      II18NService i18NService,
      IBotInputChat chat
    ) {
      _cartService = cartService;
      _routeValues = routeValues;
      _chatService = chatService;
      _orderService = orderService;
      _viewModelService = viewModelService;
      _i18NService = i18NService;
      _chat = chat;
    }

    [BotMessageAction(".*", "order/address$")]
    public string AcceptAddress(IBotInputMessage message) => $"order/address/{message.Content}";

    [BotMessageAction(".*", "order/address/{address}$")]
    public string AcceptPhone(IBotInputMessage message) => $"order/phone/${message.Content}";


    [BotQueryAction("order")]
    public object StartOrderProcessing() => _cartService.IsCartEmpty(_chat)
      ? CreateEmptyOrderMenu()
      : CreateRequestDeliveryMenu();


    [BotQueryAction("order/address")]
    public object InputAddress() => new StringViewModel(
      _i18NService["headers.request-address"],
      _chatService.FindChat(_chat).Address
    );


    [BotQueryAction("order/address/{address}")]
    public object AcceptShippingAddress() {
      _chatService.UpdateChatAddress(_chat, _routeValues.GetValue("address"));

      return new StringViewModel(
        _i18NService["headers.request-phone"],
        _chatService.FindChat(_chat).Phone
      );
    }


    [BotQueryAction("order/phone/{phone}")]
    public object AcceptPhone() {
      _chatService.UpdateChatPhone(_chat, _routeValues.GetValue("phone"));
      _orderService.SaveOrder(_chat);
      _cartService.ClearCart(_chat);

      return _viewModelService.GetTranslatedMenuList(
        "headers.order-complete",
        ("headers.main-menu", "menu")
      );
    }

    private IBotViewModel CreateRequestDeliveryMenu() => _viewModelService.GetTranslatedMenu(
      "headers.request-delivery",
      new[] {
        new[] {
          ("buttons.yes", "order/address", new Values())
        },
        new[] {
          ("buttons.no", "order/phone", new Values())
        }
      }
    );

    private IBotViewModel CreateEmptyOrderMenu() => _viewModelService.GetTranslatedMenuList(
      "headers.empty-order",
      ("buttons.main-menu", "menu")
    );
  }
}
