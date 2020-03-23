namespace ImportShopBot.Constants {
  public static class Queries {
    public const string Separator = "/";
    public const string StartInterpolation = "{";
    public const string EndInterpolation = "}";
    public const string AnySequence = ".*";
    public const string Feedback = "feedback";
    public const string Cart = "cart";
    public const string Categories = "categories";
    public const string Types = "types";
    public const string Products = "products";
    public const string MainMenu = "menu";
    public const string Order = "order";
    public const string InputShippingAddress = "order/shipping";
    public const string CompleteOrderPhone = "order/phone";
    public const string AddToCart = "cart/add";
    public const string RemoveFromCart = "cart/remove";
    public const string Start = "/start";
    
    public static string ProductsPageQuery(string category, string type, int page) => $"products/{category}/{type}/{page}";
    public static string AddToCartQuery(int productId) => $"{AddToCart}/{productId}";
    public static string RemoveFromCartQuery(int cartItemId) => $"{RemoveFromCart}/{cartItemId}";
    public static string TypesListQuery(string category) => $"types/{category}";

  }
}