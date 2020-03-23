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
    public async Task OpenCategoryList() {
      var products = await ProductService.GetProducts();

      var categoryList = products
        .GetGroups(p => p.Category)
        .ToDictionary(c => c, Queries.TypesListQuery)
        .ToInlineKeyboardColumn();

      await ReplyService.SendTextAsync(Labels.ChooseCategory, categoryList);
    }

    [UsedImplicitly]
    [QueryHandler(
      Queries.Types +
      Queries.Separator +
      Queries.StartInterpolation +
      Variables.Category +
      Queries.EndInterpolation
    )]
    public async Task OpenTypeList() {
      var category = RouteValues.GetValue(Variables.Category);
      var products = await ProductService.ByCategoryAsync(category);

      var productTypesList = products
        .GetGroups(p => p.Type)
        .ToDictionary(
          t => t,
          type => Queries.ProductsPageQuery(category, type, 0)
        )
        .ToInlineKeyboardColumn();

      await ReplyService.SendTextAsync(Labels.ChooseType, productTypesList);
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
    public async Task OpenProductList() {
      var category = RouteValues.GetValue(Variables.Category);
      var type = RouteValues.GetValue(Variables.Type);
      var page = RouteValues.GetValue(Variables.Page).ParseInt();

      await PaginateProductList(category, type, page);
    }

    private const int ProductPageLimit = 3;

    private async Task PaginateProductList(string category, string type, int page) {
      var productPage = await ProductService.PaginateAsync(category, type, page, ProductPageLimit);
      var sendProductTasks = productPage.Items.SkipLast(1).Select(SendProduct).ToList();

      await Task.WhenAll(sendProductTasks);
      await SendProductWithPaginationMarkup(
        productPage.Items.Last(),
        category,
        type,
        productPage
      );
    }

    private Task SendProduct(Product product) => ReplyService.SendMediaAsync(
      product.MediaUrl.ToInputMedia(),
      product.ToProductCaption(),
      Markups.AddToCartButton(product.Id)
    );

    private Task SendProductWithPaginationMarkup(
      Product product, string category, string type, PaginateResult<Product> paginateResult
    ) {
      var paginationMarkup = Markups.PaginationKeyboard(
        category,
        type,
        paginateResult.Page,
        !paginateResult.IsFirstPage,
        !paginateResult.IsLastPage
      );

      return ReplyService.SendMediaAsync(
        product.MediaUrl.ToInputMedia(),
        product.ToProductCaption(),
        Markups.AddToCartButton(product.Id)
          .Append(paginationMarkup)
          .Append(Markups.ToMainMenuButton)
      );
    }
  }
}