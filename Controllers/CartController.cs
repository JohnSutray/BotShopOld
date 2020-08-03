using System.Collections.Generic;
using System.Linq;
using BotCore.Attributes;
using BotCore.Interfaces;
using BotShop.Extensions;
using BotShop.Models.ViewModels;
using BotShop.Services;
using BotShop.Services.I18N;
using BotShopCore.Extensions;
using BotShopCore.Models.Entities;
using Microsoft.AspNetCore.Routing;

namespace BotShop.Controllers {
  public class CartController {
    private readonly ProductService _productService;
    private readonly RouteValueDictionary _routeValues;
    private readonly CartService _cartService;
    private readonly IBotInputChat _chat;
    private readonly II18NService _i18NService;
    private readonly ViewModelService _viewModelService;

    public CartController(
      ProductService productService,
      CartService cartService,
      II18NService i18NService,
      ViewModelService viewModelService,
      RouteValueDictionary routeValues,
      IBotInputChat chat
    ) {
      _productService = productService;
      _routeValues = routeValues;
      _cartService = cartService;
      _chat = chat;
      _i18NService = i18NService;
      _viewModelService = viewModelService;
    }

    [BotQueryAction("cart")]
    public IBotViewModel ShowCart() => _cartService.IsCartEmpty(_chat)
      ? CreateEmptyCartMenu()
      : CreateProductCardPage();

    [BotQueryAction("cart/add/{productId}", clearDisplayBeforeHandle: false)]
    public object AddToCart() {
      var product = _productService.ById(_routeValues.GetValue("productId").ParseInt());
      _cartService.AddToCart(_chat, product.Id);

      return new StringViewModel(
        _i18NService["notifications.product-added-to-cart", new Values { ["name"] = product.Name }]
      );
    }

    [BotQueryAction("cart/remove/{cartItemId}")]
    public object RemoveFromCart() {
      _cartService.RemoveFromCart(_routeValues.GetValue("cartItemId").ParseInt());

      return ShowCart();
    }

    private IBotViewModel CreateEmptyCartMenu() => _viewModelService.GetTranslatedMenuList(
      "headers.empty-cart",
      ("buttons.main-menu", "menu")
    );

    private IBotViewModel CreateProductCardPage() => new CardPageView(
      _i18NService["headers.cart"],
      GetProductCards(),
      CreateProductCardPageActions()
    );

    private CardViewModel[] GetProductCards() => _productService
      .GetProductsById(_cartService.GetCartItems(_chat).Select(cart => cart.Id))
      .Select(CardFromProduct)
      .ToArray();

    // TODO: trim description text

    private CardViewModel CardFromProduct(Product product) => new CardViewModel(
      product.Name,
      product.Description,
      product.MediaUrl,
      CreateProductCardActions(product.Id)
    );

    private IEnumerable<ButtonViewModel> CreateProductCardPageActions() => new[] {
      _viewModelService.GetTranslatedButton("buttons.main-menu", "menu")
    };

    private IEnumerable<ButtonViewModel> CreateProductCardActions(int productId) => new[] {
      _viewModelService.GetTranslatedButton("buttons.remove-from-cart", $"cart/remove/{productId}")
    };
  }
}
