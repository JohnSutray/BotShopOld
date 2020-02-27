using System;
using System.Linq;
using System.Threading.Tasks;
using ImportShopCore.Attributes;
using ImportShopCore.Enums;
using ImportShopCore.Extensions.Media;
using ImportShopCore.Models.Account;
using ImportShopCore.Models.Telegram;
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

    public async Task<Message> SendTextWithMarkupAsync(string text, IReplyMarkup markup) =>
      await SaveMessageAsync(
        await Client.SendTextMessageAsync(User.Id, text, replyMarkup: markup)
      );

    private async Task<Message> SendPhotoWithMarkupAsync(
      InputOnlineFile file, string caption, IReplyMarkup markup
    ) => await SaveMessageAsync(
      await Client.SendPhotoAsync(User.Id, file, caption, replyMarkup: markup)
    );


    private async Task<Message> SendVideoWithMarkupAsync(
      InputOnlineFile video, string caption, IReplyMarkup markup
    ) => await SaveMessageAsync(
      await Client.SendVideoAsync(User.Id, video, caption: caption, replyMarkup: markup)
    );

    public async Task<Message> SendMediaWithMarkupAsync(
      InputOnlineFile imageOrVideo, string caption, IReplyMarkup markup
    ) => imageOrVideo.Url.GetDisplayType() switch {
      EDisplayType.Video => await SendVideoWithMarkupAsync(imageOrVideo, caption, markup),
      EDisplayType.Image => await SendPhotoWithMarkupAsync(imageOrVideo, caption, markup),
      _ => throw new ArgumentOutOfRangeException()
    };

    public async Task<Message> SaveMessageAsync(Message message) {
      await MessageService.SaveMessageAsync(message);

      return message;
    }

    private Task DeleteMessage(TelegramMessage telegramMessage) => Client.DeleteMessageAsync(
      User.Id,
      telegramMessage.Id
    );
  }
}