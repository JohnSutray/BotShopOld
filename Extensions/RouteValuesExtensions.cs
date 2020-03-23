using Microsoft.AspNetCore.Routing;

namespace ImportShopBot.Extensions {
  public static class RouteValuesExtensions {
    public static string GetValue(
      this RouteValueDictionary routeValueDictionary,
      string param
    ) => routeValueDictionary[param] as string;
  }
}