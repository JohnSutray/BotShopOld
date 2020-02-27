using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ImportShopBot.Attributes;
using ImportShopBot.Models;

namespace ImportShopBot.Extensions {
  public static class MethodInfoExtensions {
    public static bool HasAttribute<TAttribute>(this MethodInfo methodInfo) where TAttribute : Attribute
      => methodInfo.GetCustomAttributes().Any(a => a is TAttribute);

    public static Models.ControllerAction<string> ToQueryAction(this MethodInfo method, Type controller) {
      var attribute = method.GetCustomAttribute<QueryHandler>();

      return new Models.ControllerAction<string>(
        controller,
        method,
        attribute.Template,
        attribute.ClearDisplayBeforeHandle
      );
    }
    
    public static Models.ControllerAction<Regex> ToMessageAction(this MethodInfo method, Type controller) {
      var attribute = method.GetCustomAttribute<MessageHandler>();

      return new Models.ControllerAction<Regex>(
        controller,
        method,
        attribute.HandlerRegex,
        attribute.ClearDisplayBeforeHandle
      );
    }
  }
}