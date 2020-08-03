using System.Collections.Generic;
using System.Linq;
using BotCore.Attributes;
using BotShop.Extensions;
using BotShop.Models.ViewModels;
using BotShop.Services;
using BotShop.Services.I18N;
using BotShopCore.Extensions;
using BotShopCore.Extensions.Common;
using BotShopCore.Models.Entities;
using Microsoft.AspNetCore.Routing;

namespace BotShop.Controllers {
  public class ProductController {
    private const int ProductPageLimit = 3;
    private readonly ProductService _productService;
    private readonly RouteValueDictionary _routeValues;
    private readonly ViewModelService _viewModelService;

    public ProductController(
      ProductService productService,
      RouteValueDictionary routeValues,
      ViewModelService viewModelService
    ) {
      _productService = productService;
      _routeValues = routeValues;
      _viewModelService = viewModelService;
    }

    [BotQueryAction("categories")]
    public object OpenCategoryList(II18NService i18NService) => new MenuViewModel(
      i18NService["headers.choose-category"],
      _productService.GetProducts()
        .GetGroups(p => p.Category)
        .Select(CreateCategoryItem)
    );

    private IEnumerable<ButtonViewModel> CreateCategoryItem(string category) => new[] {
      new ButtonViewModel(category, $"types/{category}")
    };

    [BotQueryAction("types/{category}")]
    public object OpenTypeList(II18NService i18NService) => new MenuViewModel(
      i18NService["headers.choose-type"],
      _productService.GetProducts()
        .GetGroups(p => p.Type)
        .Select(CreateTypeListItem)
    );

    private IEnumerable<ButtonViewModel> CreateTypeListItem(string type) => new[] {
      new ButtonViewModel(
        type,
        $"products/{_routeValues.GetValue("category")}/{type}/0"
      ),
    };

    [BotQueryAction("products/{category}/{type}/{page}")]
    public object OpenProductList(II18NService i18NService) {
      var category = _routeValues.GetValue("category");
      var type = _routeValues.GetValue("type");
      var page = _routeValues.GetValue("page").ParseInt();
      var productPage = _productService.Paginate(category, type, page, ProductPageLimit);
      var cards = productPage.Items.Select(CreateCardFromProduct);
      var previousPageButton = page > 0
        ? CreatePaginationButton(category, type, page - 1, true)
        : null;
      var nextPageButton = page < productPage.Limit
        ? CreatePaginationButton(category, type, page + 1, false)
        : null;

      return new CardPageView(
        i18NService["headers.select-products"],
        cards,
        new[] { previousPageButton, nextPageButton }
      );
    }

    private ButtonViewModel CreatePaginationButton(
      string category, string type, int page, bool previous
    ) => _viewModelService.GetTranslatedButton(
      $"buttons.{(previous ? "previous" : "next")}-page",
      $"products/{category}/{type}/{page}",
      new Values { ["page"] = $"{page}" }
    );

    private CardViewModel CreateCardFromProduct(Product product) => new CardViewModel(
      product.Name,
      product.Description,
      product.MediaUrl,
      new[] {
        _viewModelService.GetTranslatedButton(
          "buttons.add-to-cart",
          $"cart/add/${product.Id}"
        )
      }
    );
  }
}
