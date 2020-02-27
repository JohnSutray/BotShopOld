using System;
using System.Reflection;

namespace ImportShopBot.Models {
  public class ControllerAction<THandleBy> {
    public bool ClearDisplayBeforeHandle { get; }
    public THandleBy HandleBy { get; }
    public MethodInfo Handler { get; }
    public Type Controller { get; }

    public ControllerAction(
      Type controller,
      MethodInfo handler,
      THandleBy handleBy,
      bool clearDisplayBeforeHandle
    ) {
      HandleBy = handleBy;
      ClearDisplayBeforeHandle = clearDisplayBeforeHandle;
      Handler = handler;
      Controller = controller;
    }
  }
}