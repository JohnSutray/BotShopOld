using System.Collections.Generic;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Entities;

namespace ImportShopBot.Services {
  [Service]
  public class ProductService : RepositoryService<Product> {
    private Account Account { get; }

    public ProductService(ApplicationContext context, Account account)
      : base(context, c => c.Products) {
      Account = account;
    }

    public new Product ById(int productId) => base.ById(productId);

    public IEnumerable<Product> GetProducts() => ByPatternMany(
      product => product.AccountId == Account.Id
    );

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