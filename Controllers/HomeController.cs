using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using ImportShopBot.Services;
using JetBrains.Annotations;

namespace ImportShopBot.Controllers {
  public class HomeController {
    private ReplyService ReplyService { get; }

    public HomeController(ReplyService replyService) => ReplyService = replyService;

    [UsedImplicitly]
    [QueryHandler(Queries.MainMenu)]
    public async Task Menu() => await ReplyService.SendTextAsync(Labels.MainMenu, Markups.MainMenuKeyboard);

    [UsedImplicitly]
    [MessageHandler(Queries.Start, Queries.AnySequence)]
    public string MapMenu() => Queries.MainMenu;
  }
}