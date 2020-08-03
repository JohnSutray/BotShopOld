using System.Collections.Generic;
using System.Linq;
using BotShop.Models;
using BotShop.Models.ViewModels;
using BotShop.Services.I18N;
using BotShopCore.Attributes;

namespace BotShop.Services {
  [Service]
  public class ViewModelService {
    private readonly II18NService _ii18NService;

    public ViewModelService(II18NService ii18NService) => _ii18NService = ii18NService;

    public ButtonViewModel GetTranslatedButton(string path, string query, Values values = null) {
      return new ButtonViewModel(
        _ii18NService[path, values],
        query
      );
    }

    public MenuViewModel GetTranslatedMenuList(
      string headerTranslationPath,
      params (string translationPath, string query)[] column
    ) => GetTranslatedMenu(
      headerTranslationPath,
      column.Select(element => new[] {
        (element.translationPath, element.query, new Values())
      })
    );

    public MenuViewModel GetTranslatedMenu(
      string header,
      IEnumerable<IEnumerable<(string path, string query, Values values)>> columns
    ) {
      return new MenuViewModel(
        header,
        columns.Select(
          row => row.Select(
            button => new ButtonViewModel(
              _ii18NService[button.path, button.values],
              button.query
            )
          )
        )
      );
    }
  }
}
