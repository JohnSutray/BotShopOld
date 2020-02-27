using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Extensions.Markup {
  public static class InlineQueryMarkupExtensions {
    public static InlineKeyboardMarkup Append(this InlineKeyboardMarkup markup, InlineKeyboardMarkup extraMarkup) =>
      new InlineKeyboardMarkup(
        markup.InlineKeyboard.Concat(extraMarkup.InlineKeyboard)
      );
  }
}