using BotCore.Interfaces;
using Telegram.Bot.Types;

namespace BotShop.Telegram {
  public class TelegramInputChat : IBotInputChat {
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }

    public string PlatformId => "telegram";

    public TelegramInputChat(User user) {
      Id = user.Id;
      FirstName = user.FirstName ?? user.Username;
      LastName = user.LastName ?? "Без фамилии";
    }
  }
}
