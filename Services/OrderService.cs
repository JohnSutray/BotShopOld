using System;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;

namespace ImportShopBot.Services {
  [Service]
  public class OrderService : RepositoryService<Order> {
    private ChatService ChatService { get; }
    private OrderItemService OrderItemService { get; }
    private Account Account { get; }

    public OrderService(
      ApplicationContext context,
      ChatService chatService,
      OrderItemService orderItemService,
      Account account
    ) : base(context, c => c.Orders) {
      ChatService = chatService;
      OrderItemService = orderItemService;
      Account = account;
    }

    public void SaveOrder() {
      var chat = ChatService.GetCurrentChat();
      var orderDto = new Order {
        ChatId = chat.Id, 
        CreatedAt = DateTime.Now,
        AccountId = Account.Id
      };
      var order = AddEntity(orderDto);

      OrderItemService.SaveOrderItems(order.Id);
    }
  }
}