using System.Linq;
using BotCore.Interfaces;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotShop.Services {
  [Service]
  public class OrderItemService : RepositoryService<OrderItem> {
    private readonly CartService _cartService;

    public OrderItemService(ApplicationContext context, CartService cartService)
      : base(context) => _cartService = cartService;

    public void SaveOrderItems(IBotInputChat chat, int orderId) {
      var cartItems = _cartService.GetCartItems(chat);
      var orderItems = cartItems.Select(item => new OrderItem {
        OrderId = orderId,
        ProductId = item.ProductId
      });

      AddRange(orderItems);
    }

    protected override DbSet<OrderItem> Set => Context.OrderItems;
  }
}
