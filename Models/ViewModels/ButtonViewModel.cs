using BotCore.Interfaces;

namespace BotShop.Models.ViewModels {
  public class ButtonViewModel: IBotViewModel {
    public ButtonViewModel(string label, string query) {
      Label = label;
      Query = query;
    }

    public string Label { get; }
    public string Query { get; }
  }
}
