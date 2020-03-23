using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Extensions.Markup {
  public static class StringExtensions {
    public static KeyboardButton ToKeyboardButton(this string value) => new KeyboardButton(value);

    public static InlineKeyboardButton ToInlineButton(
      this string text, string query
    ) => new InlineKeyboardButton { Text = text, CallbackData = query };

    public static InputOnlineFile ToInputMedia(this string value) => new InputOnlineFile(value);
  }
}