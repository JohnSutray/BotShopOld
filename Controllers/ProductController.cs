using System;
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
using ImportShopCore.Models.Product;
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

    [QueryHandler("categories")]
    public async Task CategoryList() {
      var products = await ProductService.GetProducts();

      var categoryList = products
        .GetGroups(p => p.Category)
        .ToDictionary(
          c => c,
          QueryConstants.TypesList
        )
        .ToInlineKeyboardColumn();

      await ReplyService.SendTextWithMarkupAsync(TmLabelsConstants.ChooseCategory, categoryList);
    }

    [QueryHandler("types/{category}")]
    public async Task TypeList() {
      Console.WriteLine("123");
      var category = RouteValues["category"] as string;
      var products = await ProductService.ByCategoryAsync(category);

      Console.WriteLine(456);

      var productTypesList = products
        .GetGroups(p => p.Type)
        .ToDictionary(
          t => t,
          type => QueryConstants.Products(category, type, 0)
        )
        .ToInlineKeyboardColumn();

      await ReplyService.SendTextWithMarkupAsync(TmLabelsConstants.ChooseType, productTypesList);
    }

    [QueryHandler("products/{category}/{type}/{page}")]
    public async Task ToProductList() {
      var category = RouteValues["category"] as string;
      var type = RouteValues["type"] as string;
      var page = (RouteValues["page"] as string).ParseInt();

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

    private Task SendProduct(Product product) => ReplyService.SendMediaWithMarkupAsync(
      product.MediaUrl.ToInputMedia(),
      product.ToProductCaption(),
      TmMarkupConstants.AddToCartButton(product.Id)
    );

    private Task SendProductWithPaginationMarkup(
      Product product, string category, string type, PaginateResult<Product> paginateResult
    ) {
      var paginationMarkup = TmMarkupConstants.PaginationKeyboard(
        category,
        type,
        paginateResult.Page,
        !paginateResult.IsFirstPage,
        !paginateResult.IsLastPage
      );

      return ReplyService.SendMediaWithMarkupAsync(
        product.MediaUrl.ToInputMedia(),
        product.ToProductCaption(),
        TmMarkupConstants.AddToCartButton(product.Id)
          .Append(paginationMarkup)
          .Append(TmMarkupConstants.ToMainMenuButton)
      );
    }
  }
}