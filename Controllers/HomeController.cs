using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using ImportShopBot.Extensions;
using ImportShopBot.Models;
using ImportShopBot.Services;

namespace ImportShopBot.Controllers {
  public class HomeController {
    private ReplyService ReplyService { get; }
    
    public HomeController(ReplyService replyService) => ReplyService = replyService;

    [QueryHandler("menu")]
    public async Task<bool> Menu() {
      await ReplyService.SendTextWithMarkupAsync("Главное меню", TmMarkupConstants.MainMenuKeyboard);

      return true;
    }

    [MessageHandler("/start")]
    public string MapMenu() {
      return "menu";
    }
  }
}