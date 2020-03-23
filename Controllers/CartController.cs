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
    public async Task ShowCart() {
      var cartItems = await CartService.GetCartItemsAsync();

      if (!cartItems.Any()) {
        await SendEmptyCart();
        return;
      }

      foreach (var cartItem in cartItems.SkipLast(1)) {
        await SendCartItem(cartItem);
      }

      await SendCartItemWithMainMenuButton(cartItems.Last());
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
    public async Task AddToCart() {
      var productId = RouteValues.GetValue(Variables.ProductId).ParseInt();
      var product = await ProductService.ByIdAsync(productId);
      await CartService.AddToCartAsync(productId);

      await ReplyService.SendTextAsync(Labels.AddedToCart(product.Name));
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.RemoveFromCart +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.CartItemId +
      Queries.EndInterpolation
    )]
    public async Task RemoveFromCart() {
      var cartItemId = RouteValues.GetValue(Variables.CartItemId).ParseInt();
      await CartService.RemoveFromCartAsync(cartItemId);

      await ShowCart();
    }

    private async Task SendEmptyCart() => await ReplyService.SendTextAsync(
      Labels.CartIsEmpty,
      Markups.ToMainMenuButton.ToInlineKeyboard()
    );

    private async Task SendCartItem(CartItem cartItem) => await ReplyService.SendMediaAsync(
      cartItem.Product.MediaUrl.ToInputMedia(),
      cartItem.Product.ToShortProductCaption(),
      Markups.RemoveFromCartButton(cartItem.Id)
    );

    private async Task SendCartItemWithMainMenuButton(CartItem cartItem) => await ReplyService.SendMediaAsync(
      cartItem.Product.MediaUrl.ToInputMedia(),
      cartItem.Product.ToShortProductCaption(),
      Markups.RemoveFromCartButton(cartItem.Id).Append(Markups.ToMainMenuButton)
    );
  }
}