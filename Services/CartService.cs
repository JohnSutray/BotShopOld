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

    public async Task AddToCartAsync(int productId) => await AddEntityAsync(new CartItem {
      ChatId = User.Id,
      ProductId = productId
    });

    public async Task<List<CartItem>> GetCartItemsAsync() => await ByPatternManyAsync(
      item => item.ChatId == User.Id,
      item => item.Product
    );

    public async Task RemoveFromCartAsync(int cartItemId) => await RemoveByIdAsync(cartItemId);

    public async Task<bool> IsCartEmpty() {
      var itemsAmount = await CountAsync(item => item.ChatId == User.Id);

      return itemsAmount == 0;
    }
  }
}