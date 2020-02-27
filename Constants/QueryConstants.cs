namespace ImportShopBot.Constants {
  public static class QueryConstants {
    public const string Feedback = "feedback";
    public const string Cart = "cart";
    public const string Add = "add";
    public const string Categories = "categories";
    public static string Types(string category) => $"types/{category}";
    public static string Products(string category, string type, int page) => $"products/{category}/{type}/{page}";
    public static string AddProduct(int productId) => $"{Cart}/{Add}/{productId}";
    public static string TypesList(string category) => $"types/{category}";
    public const string MainMenu = "menu";
    public const string Checkout = "checkout";
  }
}