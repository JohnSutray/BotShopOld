using BotCore.Interfaces;

namespace BotShop.Models.ViewModels {
  public class StringViewModel: IBotViewModel {
    public string Value { get; }
    public string Tip { get; }

    public StringViewModel(string value, string tip = null) {
      Value = value;
      Tip = tip;
    }
  }
}
