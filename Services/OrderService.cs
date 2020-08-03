using System;
using BotCore.Interfaces;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotShop.Services {
  [Service]
  public class OrderService : RepositoryService<Order> {
    private readonly ChatService _chatService;
    private readonly OrderItemService _orderItemService;
    private readonly Account _account;

    public OrderService(
      ApplicationContext context,
      ChatService chatService,
      OrderItemService orderItemService,
      Account account
    ) : base(context) {
      _chatService = chatService;
      _orderItemService = orderItemService;
      _account = account;
    }

    public void SaveOrder(IBotInputChat inputChat) {
      var chat = _chatService.FindChat(inputChat);
      var orderDto = new Order {
        ChatId = chat.Id,
        CreatedAt = DateTime.Now,
        AccountId = _account.Id
      };
      var order = AddEntity(orderDto);

      _orderItemService.SaveOrderItems(inputChat, order.Id);
    }

    protected override DbSet<Order> Set => Context.Orders;
  }
}
