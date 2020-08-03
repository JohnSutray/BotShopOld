using System.Collections.Generic;

namespace BotShop.Services.I18N {
  public class I18NOptions {
    public string BasePath { get; set; }
    public string DefaultLanguage { get; set; }
    public IEnumerable<string> Languages { get; set; }
  }
}
