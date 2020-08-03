using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BotShop.Services.I18N {
  public class I18NService : II18NService {
    private readonly IDictionary<string, JObject> _translations = new Dictionary<string, JObject>();
    private string _currentLanguage;

    private JObject Translation => _translations[_currentLanguage];

    public string this[string path, Dictionary<string, string> values = null] => Translate(path, values);

    public string Translate(string path, Dictionary<string, string> values = null) => values?.Keys.Aggregate(
      Translation.SelectToken(path)?.Value<string>() ?? path,
      (translation, key) => translation.Replace($"{{{key}}}", values[key])
    );

    public void SetLanguage(string language) => _currentLanguage = language;

    public void ConfigureLanguages(string baseDirectory, IEnumerable<string> languages) => languages.ToList()
      .ForEach(language => _translations.Add(
          language,
          JObject.Parse(LoadJsonFile(baseDirectory, language))
        )
      );

    private string LoadJsonFile(string baseDirectory, string language) => File.ReadAllText(
      Path.Combine(Directory.GetCurrentDirectory(), baseDirectory, $"{language}.json")
    );
  }
}
