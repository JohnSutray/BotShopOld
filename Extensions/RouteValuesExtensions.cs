using Microsoft.AspNetCore.Routing;

namespace BotShop.Extensions {
  public static class RouteValuesExtensions {
    public static string GetValue(
      this RouteValueDictionary routeValueDictionary,
      string param
    ) => routeValueDictionary[param] as string;
  }
}