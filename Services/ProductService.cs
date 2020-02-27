﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ImportShopCore;
using ImportShopCore.Attributes;
using ImportShopCore.Models;
using ImportShopCore.Models.Account;
using ImportShopCore.Models.Product;

namespace ImportShopBot.Services {
  [Service]
  public class ProductService : RepositoryService<Product> {
    private Account Account { get; }

    public ProductService(ApplicationContext context, Account account)
      : base(context, c => c.Products) {
      Account = account;
    }

    public async Task<IEnumerable<Product>> GetProducts() {
      return await ByPatternManyAsync(product => product.Id == Account.Id);
    }

    public async Task<List<Product>> ByCategoryAsync(string category) => await ByPatternManyAsync(
      product => product.AccountId == Account.Id && product.Category == category
    );

    public async Task<PaginateResult<Product>> PaginateAsync(
      string category,
      string type,
      int page,
      int limit
    ) => await PaginateByPatternAsync(
      product => product.AccountId == Account.Id && product.Category == category && product.Type == type,
      page,
      limit
    );
  }
}