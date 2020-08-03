using System;
using System.Collections.Generic;
using BotCore.Interfaces;
using BotShopCore;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotShop.Services.PlatformMessageService {
  public abstract class MessageService : RepositoryService<BotMessage>, IBotMessageService {
    protected MessageService(ApplicationContext context) : base(context) {
    }

    protected override DbSet<BotMessage> Set => Context.Messages;

    protected abstract void RemoveMessagesInPlatform(IBotInputChat chat, IEnumerable<BotMessage> messages);

    private void RemoveMessage(IBotInputChat chat, BotMessage message) {
      try {
        RemoveMessagesInPlatform(chat, new[] {message});
        RemoveByPattern(m => m.Id == message.Id && m.PlatformId == chat.PlatformId);
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }
    }

    public void ClearMessages(IBotInputChat chat) => ByPatternMany(
      message => message.ChatId == chat.Id && message.PlatformId == chat.PlatformId
    ).ForEach(messageModel => RemoveMessage(chat, messageModel));

    public void SaveMessage(IBotInputChat chat, IBotInputMessage message) {
      if (ById(message.Id) == null) {
        AddEntity(
          new BotMessage {
            Id = message.Id,
            ChatId = chat.Id,
            PlatformId = chat.PlatformId
          }
        );
      }
    }
  }
}
