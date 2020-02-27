using System;
using System.Text.RegularExpressions;

namespace ImportShopBot.Attributes {
  [AttributeUsage(AttributeTargets.Method)]
  public class MessageHandler : BaseHandler {
    public Regex HandlerRegex { get; }

    public MessageHandler(
      string regex = null,
      bool clearDisplayBeforeHandle = true
    ) : base(clearDisplayBeforeHandle) => HandlerRegex = regex != null
      ? new Regex(regex)
      : new Regex(".*");
  }
}