using System;
using System.Reflection;

namespace ImportShopBot.Models {
  public class ControllerAction<TRoutingData> {
    public bool ClearDisplayBeforeHandle { get; }
    public TRoutingData RoutingData { get; }
    public MethodInfo Handler { get; }
    public Type Controller { get; }

    public ControllerAction(
      Type controller,
      MethodInfo handler,
      TRoutingData routingData,
      bool clearDisplayBeforeHandle
    ) {
      RoutingData = routingData;
      ClearDisplayBeforeHandle = clearDisplayBeforeHandle;
      Handler = handler;
      Controller = controller;
    }
  }
}