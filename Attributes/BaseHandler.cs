using System;

namespace ImportShopBot.Attributes {
  [AttributeUsage(AttributeTargets.Method)]
  public class BaseHandler : Attribute {
    public bool ClearDisplayBeforeHandle { get; }

    protected BaseHandler(bool clearDisplayBeforeHandle) => ClearDisplayBeforeHandle = clearDisplayBeforeHandle;
  }
}