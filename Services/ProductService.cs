using System.Collections.Generic;
using BotShopCore;
using BotShopCore.Attributes;
using BotShopCore.Models;
using BotShopCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BotShop.Services {
  [Service]
  public class ProductService : RepositoryService<Product> {
    private Account Account { get; }

    public ProductService(ApplicationContext context, Account account)
      : base(context) {
      Account = account;
    }

    protected override DbSet<Product> Set => Context.Products;
    public new Product ById(int productId) => base.ById(productId);

    public IEnumerable<Product> GetProducts() => ByPatternMany(
      product => product.AccountId == Account.Id
    );

    public IEnumerable<Product> GetProductsById(IEnumerable<int> ids) => ByIdMany(ids);

    public List<Product> ByCategory(string category) => ByPatternMany(
      product => product.AccountId == Account.Id && product.Category == category
    );

    public PaginationResult<Product> Paginate(
      string category,
      string type,
      int page,
      int limit
    ) => PaginateByPattern(
      product => product.AccountId == Account.Id && product.Category == category && product.Type == type,
      page,
      limit
    );
  }
}
