using Microsoft.Extensions.DependencyInjection;

namespace BotShop.Services.I18N {
  public static class I18NExtensions {
    public static IServiceCollection AddI18NService(
      this IServiceCollection collection, I18NOptions options
    ) => collection.AddSingleton<II18NService>(_ => {
      var service = new I18NService();

      service.ConfigureLanguages(options.BasePath, options.Languages);
      service.SetLanguage(options.DefaultLanguage);

      return service;
    });
  }
}
