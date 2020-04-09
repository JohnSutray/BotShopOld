using System.Linq;
using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using ImportShopBot.Extensions;
using ImportShopBot.Extensions.Markup;
using ImportShopBot.Services;
using ImportShopCore.Extensions;
using ImportShopCore.Models.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace ImportShopBot.Controllers {
  public class CartController {
    private ProductService ProductService { get; }
    private ReplyService ReplyService { get; }
    private RouteValueDictionary RouteValues { get; }
    private CartService CartService { get; }

    public CartController(
      ReplyService replyService,
      ProductService productService,
      RouteValueDictionary routeValues,
      CartService cartService
    ) {
      ReplyService = replyService;
      ProductService = productService;
      RouteValues = routeValues;
      CartService = cartService;
    }

    [UsedImplicitly]
    [QueryHandler(Queries.Cart)]
    public void ShowCart() {
      var cartItems = CartService.GetCartItems();

      if (!cartItems.Any()) {
        SendEmptyCart();
        return;
      }

      foreach (var cartItem in cartItems.SkipLast(1)) {
        SendCartItem(cartItem);
      }

      SendCartItemWithMainMenuButton(cartItems.Last());
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.AddToCart +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.ProductId +
      Queries.EndInterpolation,
      false
    )]
    public void AddToCart() {
      var productId = RouteValues.GetValue(Variables.ProductId).ParseInt();
      var product = ProductService.ById(productId);
      CartService.AddToCart(productId);
      ReplyService.SendText(Labels.AddedToCart(product.Name));
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.RemoveFromCart +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.CartItemId +
      Queries.EndInterpolation
    )]
    public void RemoveFromCart() {
      var cartItemId = RouteValues.GetValue(Variables.CartItemId).ParseInt();
      CartService.RemoveFromCartAsync(cartItemId);

      ShowCart();
    }

    private void SendEmptyCart() => ReplyService.SendText(
      Labels.CartIsEmpty,
      Markups.ToMainMenuButton.ToInlineKeyboard()
    );

    private void SendCartItem(CartItem cartItem) => ReplyService.SendMedia(
      cartItem.Product.MediaUrl.ToInputMedia(),
      cartItem.Product.ToShortProductCaption(),
      Markups.RemoveFromCartButton(cartItem.Id)
    );

    private void SendCartItemWithMainMenuButton(CartItem cartItem) => ReplyService.SendMedia(
      cartItem.Product.MediaUrl.ToInputMedia(),
      cartItem.Product.ToShortProductCaption(),
      Markups.RemoveFromCartButton(cartItem.Id).Append(Markups.ToMainMenuButton)
    );
  }
}