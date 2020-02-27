using System.Threading.Tasks;
using ImportShopCore.Extensions;
using ImportShopBot.Attributes;
using ImportShopBot.Models;
using ImportShopBot.Services;

namespace ImportShopBot.Controllers {
  public class CartController {
    public CartController(ReplyService replyService) {
      ReplyService = replyService;
    }
    private ReplyService ReplyService { get; }
    //
    // [QueryHandler("cart/add/{productId}", false)]
    // public async Task<bool> AddToCart() {
    //   // var product = await context.ProductService.ProductByIdAsync(
    //   //   context.AccountId,
    //   //   context.RouteValues["productId"].ParseInt()
    //   // );
    //   
    //   
    //
    //   return true;
    // }
  }
}