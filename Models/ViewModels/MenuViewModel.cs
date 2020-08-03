using System.Collections.Generic;
using BotCore.Interfaces;

namespace BotShop.Models.ViewModels {
  public class MenuViewModel: IBotViewModel {
    public MenuViewModel(
      string header,
      IEnumerable<IEnumerable<ButtonViewModel>> buttons
    ) {
      Header = header;
      Buttons = buttons;
    }

    public string Header { get; }
    public IEnumerable<IEnumerable<ButtonViewModel>> Buttons { get; }
  }
}
