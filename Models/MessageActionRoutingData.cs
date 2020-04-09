using System.Text.RegularExpressions;

namespace ImportShopBot.Models {
  public class MessageActionRoutingData {
    public Regex MessagePattern { get; set; }
    public Regex LatestQuery { get; set; }
    public int Priority { get; set; }
  }
}