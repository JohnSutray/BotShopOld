using System.Linq;
using System.Threading.Tasks;
using ImportShopBot.Attributes;
using ImportShopBot.Constants;
using ImportShopBot.Extensions;
using ImportShopBot.Extensions.Markup;
using ImportShopBot.Services;
using ImportShopCore.Extensions;
using ImportShopCore.Extensions.Common;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Routing;

namespace ImportShopBot.Controllers {
  public class ProductController {
    private ReplyService ReplyService { get; }
    private ProductService ProductService { get; }
    private RouteValueDictionary RouteValues { get; }

    public ProductController(
      ReplyService replyService,
      ProductService productService, RouteValueDictionary routeValues) {
      ReplyService = replyService;
      ProductService = productService;
      RouteValues = routeValues;
    }

    [UsedImplicitly]
    [QueryHandler(Queries.Categories)]
    public void OpenCategoryList() {
      var products = ProductService.GetProducts();

      var categoryList = products
        .GetGroups(p => p.Category)
        .ToDictionary(c => c, Queries.TypesListQuery)
        .ToInlineKeyboardColumn();

      ReplyService.SendText(Labels.ChooseCategory, categoryList);
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.Types +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Category +
      Queries.EndInterpolation
    )]
    public void OpenTypeList() {
      var category = RouteValues.GetValue(Variables.Category);
      var products = ProductService.ByCategory(category);

      var productTypesList = products
        .GetGroups(p => p.Type)
        .ToDictionary(
          t => t,
          type => Queries.ProductsPageQuery(category, type, 0)
        )
        .ToInlineKeyboardColumn();

      ReplyService.SendText(Labels.ChooseType, productTypesList);
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.Products +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Category +
      Queries.EndInterpolation +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Type +
      Queries.EndInterpolation +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Page +
      Queries.EndInterpolation
    )]
    public void OpenProductList() {
      var category = RouteValues.GetValue(Variables.Category);
      var type = RouteValues.GetValue(Variables.Type);
      var page = RouteValues.GetValue(Variables.Page).ParseInt();

      PaginateProductList(category, type, page);
    }

    private const int ProductPageLimit = 3;

    private void PaginateProductList(string category, string type, int page) {
      var productPage = ProductService.Paginate(category, type, page, ProductPageLimit);

      var sendProductTasks = productPage.Items.SkipLast(1).Select(SendProduct).ToList();

      Task.WhenAll(sendProductTasks).GetAwaiter().GetResult();

      SendProductWithPaginationMarkup(
        productPage.Items.Last(),
        category,
        type,
        productPage
      );
    }

    private Task SendProduct(Product product) => Task.Run(() => ReplyService.SendMedia(
      product.MediaUrl.ToInputMedia(),
      product.ToProductCaption(),
      Markups.AddToCartButton(product.Id)
    ));

    private void SendProductWithPaginationMarkup(
      Product product, string category, string type, PaginationResult<Product> paginationResult
    ) {
      var paginationMarkup = Markups.PaginationKeyboard(
        category,
        type,
        paginationResult.Page,
        !paginationResult.IsFirstPage,
        !paginationResult.IsLastPage
      );

      ReplyService.SendMedia(
        product.MediaUrl.ToInputMedia(),
        product.ToProductCaption(),
        Markups.AddToCartButton(product.Id)
          .Append(paginationMarkup)
          .Append(Markups.ToMainMenuButton)
      );
    }
  }
}