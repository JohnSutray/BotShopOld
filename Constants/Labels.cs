namespace ImportShopBot.Constants {
  public static class Labels {
    public const string Catalog = "Каталог";
    public const string Cart = "Корзина";
    public const string MainMenu = "Главное меню";
    public const string Checkout = "Оформить заказ";
    public const string Feedback = "Заказать обратный звонок";
    public const string ChooseCategory = "Выберите категорию";
    public const string ChooseType = "Выберите подкатегорию";
    public const string AddToCart = "Добавить в корзину";
    public const string RemoveFromCart = "Удалить из корзины";
    public const string ToMainMenuButton = "В главное меню";
    public const string CartIsEmpty = "В корзине пусто.";
    public const string CannotCompleteEmptyOrder = "Невозможно оформить заказ, так как в корзине нет продуктов.";
    public const string AreYouNeedShipping = "Вам нужна доставка?";
    public const string InputAddress = "Пожалуйста, введите адрес доставки.";
    public const string InputPhone = "Пожалуйста, введите свой номер телефона." +
                                     "\nТелефон может быть таким +375ххххххххх";

    public const string OrderCompleted = "Ваш заказ успешно оформлен. Мы свяжемся с вами в ближайшее время";
    public const string Yes = "Да";
    public const string No = "Нет";

    public static string PreviousPageLabel(int page) => $"Предыдущие 10 продуктов ({page} страница)";
    public static string NextPaginationLabel(int page) => $"Следующие 10 продуктов ({page} страница)";
    
    public static string AddedToCart(string productName) => $"{productName} добавлен(-о) в корзину";
  }
}