using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Extensions.Markup {
  public static class MarkupExtensions {
    public static InlineKeyboardMarkup Append(
      this InlineKeyboardMarkup markup, InlineKeyboardMarkup extraMarkup
    ) => new InlineKeyboardMarkup(
      markup.InlineKeyboard.Concat(extraMarkup.InlineKeyboard)
    );

    public static InlineKeyboardMarkup ToInlineKeyboard(this InlineKeyboardButton button) =>
      new InlineKeyboardMarkup(button);

    public static ReplyKeyboardMarkup ToKeyboard(this KeyboardButton button) =>
      new ReplyKeyboardMarkup(button);
  }
}