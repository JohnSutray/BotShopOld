using System.Collections.Generic;
using ImportShopCore.Extensions;
using ImportShopBot.Extensions.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Constants {
  public static class TmMarkupConstants {
    private static readonly InlineKeyboardButton CatalogButton = TmLabelsConstants.Catalog
      .ToInlineKeyboardButton(QueryConstants.Categories);

    private static readonly InlineKeyboardButton CartButton = TmLabelsConstants.Cart
      .ToInlineKeyboardButton(QueryConstants.Cart);

    private static readonly InlineKeyboardButton CheckoutButton = TmLabelsConstants.Checkout
      .ToInlineKeyboardButton(QueryConstants.Checkout);

    private static readonly InlineKeyboardButton FeedbackButton = TmLabelsConstants.Feedback
      .ToInlineKeyboardButton(QueryConstants.Feedback);

    public static readonly InlineKeyboardButton ToMainMenuButton = TmLabelsConstants.ToMainMenuButton
      .ToInlineKeyboardButton(QueryConstants.MainMenu);

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
        var previousPageButton = TmLabelsConstants
          .PaginationLabel(previousPage)
          .ToInlineKeyboardButton(QueryConstants.Products(category, type, previousPage));
        paginationRow.Add(previousPageButton);
      }

      if (nextExists) {
        var nextPage = page + 1;
        var nextPageButton = TmLabelsConstants
          .PaginationLabel(nextPage)
          .ToInlineKeyboardButton(QueryConstants.Products(category, type, nextPage));
        paginationRow.Add(nextPageButton);
      }

      return new InlineKeyboardMarkup(paginationRow);
    }

    public static InlineKeyboardMarkup AddToCartButton(int productId) =>
      new InlineKeyboardMarkup(
        TmLabelsConstants.AddToCart
          .ToInlineKeyboardButton(QueryConstants.AddProduct(productId))
          .WrapIntoEnumerable()
      );
  }
}