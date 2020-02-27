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

    public static MethodInfo FindQueryHandler(this Type type, string query) =>
      type.GetMethodsWithAttribute<QueryHandler>().FirstOrDefault(
        method => method.IsMatchQuery(query)
      );

    private static bool IsMatchQuery(this MethodInfo method, string query) {
      var attribute = method.GetCustomAttribute<QueryHandler>();

      return attribute.Template.MatchRoute(query) != null;
    }

    public static IEnumerable<Models.ControllerAction<string>> GetQueryHandlers(this Type controller) {
      return controller.GetMethodsWithAttribute<QueryHandler>()
        .Select(method => method.ToQueryAction(controller));
    }
    
    public static IEnumerable<Models.ControllerAction<Regex>> GetMessageHandlers(this Type controller) {
      return controller.GetMethodsWithAttribute<MessageHandler>()
        .Select(method => method.ToMessageAction(controller));
    }
  }
}