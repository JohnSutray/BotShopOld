namespace ImportShopBot.Constants {
  public static class Queries {
    public const string Separator = "/";
    public const string StartInterpolation = "{";
    public const string EndInterpolation = "}";
    public const string AnySequence = ".*";
    public const string FullMatch = "$";
    public const string Feedback = "feedback";
    public const string Cart = "cart";
    public const string Categories = "categories";
    public const string Types = "types";
    public const string Products = "products";
    public const string MainMenu = "menu";
    public const string Order = "order";
    public const string Address = "address";
    public const string Phone = "phone";
    public const string Add = "add";
    public const string Remove = "remove";
    public const string InputAddress = Order + Separator + Address;
    public const string InputPhone = Order + Separator + Phone;
    public const string AddToCart = Cart + Separator + Add;
    public const string RemoveFromCart = Cart + Separator + Remove;
    public const string AcceptAddress = InputAddress + Separator + StartInterpolation + Address + EndInterpolation;
    public const string AcceptPhone = InputPhone + Separator + StartInterpolation + Phone + EndInterpolation;

    public const string Start = "/start";

    public static string ProductsPageQuery(string category, string type, int page) =>
      $"products/{category}/{type}/{page}";

    public static string AddToCartQuery(int productId) => $"{AddToCart}/{productId}";
    public static string RemoveFromCartQuery(int cartItemId) => $"{RemoveFromCart}/{cartItemId}";
    public static string TypesListQuery(string category) => $"types/{category}";
  }
}