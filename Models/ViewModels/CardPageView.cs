using System.Collections.Generic;
using BotCore.Interfaces;

namespace BotShop.Models.ViewModels {
  public class CardPageView: IBotViewModel {
    public CardPageView(
      string header,
      IEnumerable<CardViewModel> cards,
      IEnumerable<ButtonViewModel> actions
     ) {
      Header = header;
      Cards = cards;
      Actions = actions;
    }

    public string Header { get; }
    public IEnumerable<CardViewModel> Cards { get; }
    public IEnumerable<ButtonViewModel> Actions { get; }
  }
}
