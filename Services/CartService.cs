using System.Collections.Generic;
using System.Threading.Tasks;
using BotCore.Interfaces;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotShop.Services {
  [Service]
  public class CartService : RepositoryService<CartItem> {

    public CartService(ApplicationContext context) : base(context) {}

    public void AddToCart(IBotInputChat chat, int productId) => AddEntity(new CartItem {
      ChatId = chat.Id,
      ProductId = productId
    });

    public List<CartItem> GetCartItems(IBotInputChat chat) => ByPatternMany(
      item => item.ChatId == chat.Id,
      item => item.Product
    );

    public void RemoveFromCart(int cartItemId) => RemoveById(cartItemId);

    public void ClearCart(IBotInputChat chat) => RemoveManyByPattern(item => item.ChatId == chat.Id);

    public bool IsCartEmpty(IBotInputChat chat) => Count(item => item.ChatId == chat.Id) == 0;

    protected override DbSet<CartItem> Set => Context.CartItems;
  }
}
