using System.Linq;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;

namespace ImportShopBot.Services {
  [Service]
  public class OrderItemService : RepositoryService<OrderItem> {
    private CartService CartService { get; }

    [Service]
    public OrderItemService(
      ApplicationContext context,
      CartService cartService
    ) : base(context, c => c.OrderItems) => CartService = cartService;

    public void SaveOrderItems(int orderId) {
      var cartItems = CartService.GetCartItems();
      var orderItems = cartItems.Select(item => new OrderItem {
        OrderId = orderId,
        ProductId = item.ProductId
      });

      AddRange(orderItems);
    }
  }
}