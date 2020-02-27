namespace ImportShopBot.Constants {
  public static class TmLabelsConstants {
    public const string Catalog = "Каталог";
    public const string Cart = "Корзина";
    public const string Checkout = "Оформить заказ";
    public const string Feedback = "Заказать обратный звонок";
    public const string PreviousPage = "Предыдущие 10 продуктов";
    public const string Page = "страница";
    public const string NextPage = "Следующие 10 продуктов";
    public const string ChooseCategory = "Выберите категорию";
    public const string ChooseType = "Выберите подкатегорию";
    public const string AddProductsToCart = "Добавьте продукты в корзину";
    public const string AddToCart = "Добавить в корзину";
    public const string ToMainMenuButton = "В главное меню";

    public static string PaginationLabel(int page) => $"{PreviousPage} ({page} {Page})";
  }
}