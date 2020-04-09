using System.Collections.Generic;
using ImportShopCore.Extensions;
using ImportShopBot.Extensions.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Constants {
  public static class Markups {
    private static readonly InlineKeyboardButton CatalogButton = Labels.Catalog
      .ToInlineButton(Queries.Categories);

    private static readonly InlineKeyboardButton CartButton = Labels.Cart
      .ToInlineButton(Queries.Cart);

    private static readonly InlineKeyboardButton CheckoutButton = Labels.Checkout
      .ToInlineButton(Queries.Order);

    private static readonly InlineKeyboardButton FeedbackButton = Labels.Feedback
      .ToInlineButton(Queries.Feedback);

    public static readonly InlineKeyboardButton ToMainMenuButton = Labels.ToMainMenuButton
      .ToInlineButton(Queries.MainMenu);

    public static readonly InlineKeyboardMarkup MainMenuKeyboard = new InlineKeyboardMarkup(new[] {
      CatalogButton.WrapIntoEnumerable(),
      CartButton.WrapIntoEnumerable(),
      CheckoutButton.WrapIntoEnumerable(),
      FeedbackButton.WrapIntoEnumerable()
    });

    public static InlineKeyboardMarkup PaginationKeyboard(
      string category,
      string type,
      int page,
      bool previousExists,
      bool nextExists
    ) {
      var paginationRow = new List<InlineKeyboardButton>();

      if (previousExists) {
        var previousPage = page - 1;
        var previousPageQuery = Queries.ProductsPageQuery(category, type, previousPage);
        var previousPageButton = Labels.PreviousPageLabel(previousPage).ToInlineButton(previousPageQuery);
        paginationRow.Add(previousPageButton);
      }

      if (nextExists) {
        var nextPage = page + 1;
        var nextPageQuery = Queries.ProductsPageQuery(category, type, nextPage);
        var nextPageButton = Labels.NextPaginationLabel(nextPage).ToInlineButton(nextPageQuery);
        paginationRow.Add(nextPageButton);
      }

      return new InlineKeyboardMarkup(paginationRow);
    }

    public static InlineKeyboardMarkup AddToCartButton(int productId) => Labels.AddToCart
      .ToInlineButton(Queries.AddToCartQuery(productId))
      .ToInlineKeyboard();


    public static InlineKeyboardMarkup RemoveFromCartButton(int cartItemId) => Labels.RemoveFromCart
      .ToInlineButton(Queries.RemoveFromCartQuery(cartItemId))
      .ToInlineKeyboard();

    public static readonly InlineKeyboardMarkup ShippingConfirmationKeyboard = new InlineKeyboardMarkup(new[] {
      Labels.Yes.ToInlineButton(Queries.InputAddress),
      Labels.No.ToInlineButton(Queries.InputPhone)
    });
  }
}