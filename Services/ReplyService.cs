using System;
using ImportShopCore.Attributes;
using ImportShopCore.Enums;
using ImportShopCore.Extensions.Media;
using ImportShopCore.Models.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ImportShopBot.Services {
  [Service]
  public class ReplyService {
    private MessageService MessageService { get; }
    private TelegramBotClient Client { get; }
    private Account Account { get; }
    private User User { get; }

    public ReplyService(
      TelegramBotClient client,
      Account account,
      User user,
      MessageService messageService
    ) {
      Client = client;
      Account = account;
      User = user;
      MessageService = messageService;
    }

    public Message SendText(string text, IReplyMarkup markup = null) =>
      SaveMessageAsync(
        Client.SendTextMessageAsync(User.Id, text, replyMarkup: markup).GetAwaiter().GetResult()
      );

    private Message SendPhotoWithMarkup(
      InputOnlineFile file, string caption, IReplyMarkup markup
    ) => SaveMessageAsync(
      Client.SendPhotoAsync(User.Id, file, caption, replyMarkup: markup).GetAwaiter().GetResult()
    );


    private Message SendVideoWithMarkup(
      InputOnlineFile video, string caption, IReplyMarkup markup
    ) => SaveMessageAsync(
      Client.SendVideoAsync(User.Id, video, caption: caption, replyMarkup: markup).GetAwaiter().GetResult()
    );

    public Message SendMedia(
      InputOnlineFile imageOrVideo, string caption, IReplyMarkup markup
    ) => imageOrVideo.Url.GetDisplayType() switch {
      EDisplayType.Video => SendVideoWithMarkup(imageOrVideo, caption, markup),
      EDisplayType.Image => SendPhotoWithMarkup(imageOrVideo, caption, markup),
      _ => throw new ArgumentOutOfRangeException()
    };

    public Message SaveMessageAsync(Message message) {
      MessageService.SaveMessage(message);

      return message;
    }
  }
}