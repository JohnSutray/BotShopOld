using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Extensions.Markup {
  public static class EnumerableExtensions {
    public static InlineKeyboardMarkup ToInlineKeyboardColumn(this IDictionary<string, string> items) =>
      new InlineKeyboardMarkup(
        items.Select(
          item => item.Key.ToInlineKeyboardButton(item.Value)
        )
      );
  }
}