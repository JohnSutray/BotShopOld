using System.Collections.Generic;

namespace BotShop.Services.I18N {
  public interface II18NService {
    public string Translate(string path, Dictionary<string, string> values = null);

    public string this[string path, Dictionary<string, string> values = null] {
      get;
    }
  }
}
