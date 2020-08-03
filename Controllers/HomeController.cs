using BotCore.Attributes;
using BotShop.Models.ViewModels;
using BotShop.Services;

namespace BotShop.Controllers {
  public class HomeController {
    [BotQueryAction("menu")]
    public MenuViewModel Menu(ViewModelService viewModelService) => viewModelService.GetTranslatedMenuList(
      "headers.main-menu",
      ("buttons.categories", "categories"),
      ("buttons.cart", "cart"),
      ("buttons.order", "order"),
      ("buttons.feedback", "feedback")
    );

    [BotMessageAction("/start", ".*", priority: 2)]
    public string MapMenu() => "menu";
  }
}
