using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ImportShopBot.Attributes;
using ImportShopBot.Extensions.Query;
using ImportShopBot.Models;

namespace ImportShopBot.Extensions {
  public static class TypeExtensions {
    public static IEnumerable<MethodInfo> GetMethodsWithAttribute<TAttribute>(this Type type)
      where TAttribute : Attribute =>
      type.GetMethods().Where(m => m.HasAttribute<TAttribute>());

    public static bool HasMethodsWithAttribute<TAttribute>(this Type type) where TAttribute : Attribute =>
      type.GetMethodsWithAttribute<TAttribute>().Any();

    public static bool IsQueryController(this Type controller) => 
      controller.HasMethodsWithAttribute<QueryHandler>();

    public static bool IsMessageController(this Type controller) =>
      controller.HasMethodsWithAttribute<MessageHandler>();

    private static bool IsMatchQuery(this MethodInfo method, string query) => 
      method.GetCustomAttribute<QueryHandler>()
        .Template
        .MatchRoute(query) != null;

    public static IEnumerable<ControllerAction<string>> GetQueryActions(this Type controller) {
      return controller.GetMethodsWithAttribute<QueryHandler>()
        .Select(method => method.ToQueryAction(controller));
    }

    public static IEnumerable<ControllerAction<MessageActionRoutingData>> GetMessageActions(this Type controller) {
      return controller.GetMethodsWithAttribute<MessageHandler>()
        .Select(method => method.ToMessageAction(controller));
    }
  }
}