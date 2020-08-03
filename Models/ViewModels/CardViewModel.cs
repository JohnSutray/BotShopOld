using System.Collections.Generic;
using BotCore.Interfaces;

namespace BotShop.Models.ViewModels {
  public class CardViewModel: IBotViewModel {
    public CardViewModel(
      string header,
      string content,
      string mediaUrl,
      IEnumerable<ButtonViewModel> actions
    ) {
      Header = header;
      Content = content;
      MediaUrl = mediaUrl;
      Actions = actions;
    }

    public string Header { get; }
    public string Content { get; }
    public string MediaUrl { get; }
    public IEnumerable<ButtonViewModel> Actions { get; }
  }
}
