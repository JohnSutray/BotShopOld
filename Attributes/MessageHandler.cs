using System;
using System.Text.RegularExpressions;
using ImportShopBot.Models;
using JetBrains.Annotations;

namespace ImportShopBot.Attributes {
  [AttributeUsage(AttributeTargets.Method)]
  public class MessageHandler : BaseHandler {
    public MessageActionRoutingData RoutingData { get; }

    public MessageHandler(
      [RegexPattern] string messagePattern,
      [RegexPattern] string queryPattern,
      bool clearDisplayBeforeHandle = true,
      int priority = 1
    ) : base(clearDisplayBeforeHandle) {
      RoutingData = new MessageActionRoutingData {
        MessagePattern = new Regex(messagePattern),
        LatestQuery = new Regex(queryPattern),
        Priority = priority
      };
    }
  }
}