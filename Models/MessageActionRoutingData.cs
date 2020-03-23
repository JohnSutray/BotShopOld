using System.Text.RegularExpressions;

namespace ImportShopBot.Models {
  public class MessageActionRoutingData {
    public Regex MessagePattern { get; set; }
    public Regex QueryPattern { get; set; }
  }
}