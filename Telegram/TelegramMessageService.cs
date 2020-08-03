using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotCore.Interfaces;
using BotShop.Services.PlatformMessageService;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Enums;
using BotShopCore.Extensions.Media;
using BotShopCore.Models.Entities;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotShop.Telegram {
  [Service(ResolveType = typeof(IBotMessageService))]
  [Service(ResolveType = typeof(TelegramMessageService))]
  public class TelegramMessageService : MessageService {
    private ITelegramBotClient Client { get; }

    public TelegramMessageService(ApplicationContext context, ITelegramBotClient client)
      : base(context) => Client = client;

    public void SendText(
      IBotInputChat chat, string text, IReplyMarkup markup = null
    ) => SaveMessage(
      chat,
      new TelegramInputMessage(Client.SendTextMessageAsync(chat.Id, text, replyMarkup: markup).Result)
    );


    private async Task SendPhotoAsync(
      IBotInputChat chat, InputOnlineFile photo, string caption, IReplyMarkup markup
    ) => SaveMessage(
      chat,
      new TelegramInputMessage(
        await Client.SendPhotoAsync(chat.Id, photo, caption, replyMarkup: markup)
      )
    );

    public void SendMedia(
      IBotInputChat chat, string mediaUrl, string caption, IReplyMarkup markup
    ) => SendMediaAsync(chat, mediaUrl, caption, markup).GetAwaiter().GetResult();

    private async Task SendMediaAsync(
      IBotInputChat chat, string mediaUrl, string caption, IReplyMarkup markup
    ) {
      var imageOrVideo = new InputOnlineFile(new Uri(mediaUrl));
      switch (imageOrVideo.Url.GetDisplayType()) {
        case EDisplayType.Video:
          await SendVideoAsync(chat, imageOrVideo, caption, markup);
          return;
        case EDisplayType.Image:
          await SendPhotoAsync(chat, imageOrVideo, caption, markup);
          return;
        default: throw new ArgumentOutOfRangeException();
      }
    }

    public void SendMediaBulk<TMarkup>(
      IBotInputChat chat,
      IEnumerable<(string caption, string url, TMarkup markup)> elements
    ) where TMarkup : IReplyMarkup => Task.WaitAll(
      elements.Select(element => SendMediaAsync(chat, element.url, element.caption, element.markup)).ToArray()
    );

    private async Task SendVideoAsync(
      IBotInputChat chat, InputOnlineFile video, string caption, IReplyMarkup markup
    ) => SaveMessage(
      chat,
      new TelegramInputMessage(await Client.SendPhotoAsync(chat.Id, video, caption, replyMarkup: markup))
    );

    protected override void RemoveMessagesInPlatform(
      IBotInputChat chat, IEnumerable<BotMessage> messages
    ) => Task.WhenAll(
      messages.Select(message => DeleteMessageAsync(chat.Id, message.Id))
    ).GetAwaiter().GetResult();

    private async Task DeleteMessageAsync(int chatId, int messageId) {
      try {
        await Client.DeleteMessageAsync(chatId, messageId);
      }
      catch (Exception exception) {
        Console.WriteLine(exception);
        throw;
      }
    }
  }
}
