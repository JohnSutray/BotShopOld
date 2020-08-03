using BotShopCore.Models.Entities;

namespace BotShop.Extensions {
  public static class ProductExtensions {
    public static string ToProductCaption(this Product product) => string.Join(
      '\n',
      product.Name,
      product.Description,
      $"Стоимость: {product.Price}р."
    );

    public static string ToShortProductCaption(this Product product) => string.Join(
      '\n',
      product.Name,
      product.Description.Length > 200 ? product.Description.Substring(0, 200) + "..." : product.Description,
      $"Стоимость: {product.Price}р."
      );
  }
}