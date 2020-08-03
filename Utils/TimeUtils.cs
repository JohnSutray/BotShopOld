using System;
using System.Threading;
using System.Threading.Tasks;

namespace BotShop.Utils {
  public static class TimeUtils {
    public static async void SetInterval(TimeSpan interval, Action action, CancellationToken cancellationToken = new CancellationToken()) {
      while (true) {
        await Task.Delay(interval);

        if (cancellationToken.IsCancellationRequested) {
          return;
        }

        action();
      }
    }
  }
}