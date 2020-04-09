using System.Collections.Generic;
using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;
using Telegram.Bot.Types;

namespace ImportShopBot.Services {
  [Service]
  public class CartService : RepositoryService<CartItem> {
    private User User { get; }

    public CartService(
      ApplicationContext context,
      User user
    ) : base(
      context,
      c => c.CartItems
    ) => User = user;

    public void AddToCart(int productId) => AddEntity(new CartItem {
      ChatId = User.Id,
      ProductId = productId
    });

    public List<CartItem> GetCartItems() => ByPatternMany(
      item => item.ChatId == User.Id,
      item => item.Product
    );

    public void RemoveFromCartAsync(int cartItemId) => RemoveById(cartItemId);

    public void ClearCart() => RemoveManyByPattern(item => item.ChatId == User.Id);

    public bool IsCartEmpty() {
      var itemsAmount = Count(item => item.ChatId == User.Id);

      return itemsAmount == 0;
    }
  }
}