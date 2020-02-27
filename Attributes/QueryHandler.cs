using System;

namespace ImportShopBot.Attributes {
  [AttributeUsage(AttributeTargets.Method)]
  public class QueryHandler : BaseHandler {
    public string Template { get; }

    public QueryHandler(
      string template,
      bool clearDisplayBeforeHandle = true
    ) : base(clearDisplayBeforeHandle) => Template = template;
  }
}