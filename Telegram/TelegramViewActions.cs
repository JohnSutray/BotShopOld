using System.Collections.Generic;
using System.Linq;
using BotCore.Attributes;
using BotCore.Interfaces;
using BotShop.Constants;
using BotShop.Models.ViewModels;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotShop.Telegram {
  public class MenuView {
    [BotViewAction(typeof(StringViewModel), Platforms.Telegram)]
    public void RepresentString(
      StringViewModel stringViewModel,
      TelegramMessageService messageService,
      IBotInputChat chat
    ) => messageService.SendText(
      chat,
      stringViewModel.Value,
      new ReplyKeyboardMarkup {
        Keyboard = new [] {
          new [] {
            new KeyboardButton {
              Text = stringViewModel.Tip
            },
          }
        },
        OneTimeKeyboard = true,
      }
    );


    [BotViewAction(typeof(MenuViewModel), Platforms.Telegram)]
    public void RepresentList(
      MenuViewModel menuView,
      TelegramMessageService messageService,
      IBotInputChat chat
    ) => messageService.SendText(
      chat,
      menuView.Header,
      CreateInlineKeyBoard(menuView.Buttons)
    );

    [BotViewAction(typeof(CardPageView), Platforms.Telegram)]
    public void RepresentCardPage(
      CardPageView cardPage,
      TelegramMessageService messageService,
      IBotInputChat chat
    ) {
      messageService.SendText(chat, cardPage.Header);
      messageService.SendMediaBulk(chat, cardPage.Cards.SkipLast(1).Select(CardToMediaDto));
      var lastCard = cardPage.Cards.Last();
      messageService.SendMedia(
        chat,
        lastCard.MediaUrl,
        lastCard.Content,
        CreateInlineKeyBoard(
          ArrayFrom(
            lastCard.Actions,
            cardPage.Actions
          )
        )
      );
    }

    private (string, string, IReplyMarkup) CardToMediaDto(CardViewModel card) => (
      $"{card.Header}\n{card.Content}",
      card.MediaUrl,
      CreateInlineKeyBoard(ArrayFrom(card.Actions))
    );

    private TElement[] ArrayFrom<TElement>(params TElement[] elements) => elements;

    private IReplyMarkup CreateInlineKeyBoard(
      IEnumerable<IEnumerable<ButtonViewModel>> buttons
    ) => new InlineKeyboardMarkup(
      buttons.Select(
        row => row.Select(
          button => new InlineKeyboardButton {
            Text = button.Label,
            CallbackData = button.Query
          }
        )
      )
    );
  }
}
