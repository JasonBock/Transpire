# TProxy must have a public parameterless constructor

## Issue

This analyzer is tripped if you pass a type to the `TProxy` parameter for `DispatchProxy` that does not have a public parameterless constructor:

```
public class PrivateConstructorDispatchProxy
  : DispatchProxy
{
  private PrivateConstructorDispatchProxy() { }

  // Invoke() definition goes here...
}

public interface IUsingDispatchProxy { }

var proxy = DispatchProxy.Create<IUsingDispatchProxy, PrivateConstructorDispatchProxy>();
```

`DispatchProxy` will only work with proxy types that have an accessible constructor with no arguments, but this won't be an exception until runtime.

## Code Fix

No code fix is available.