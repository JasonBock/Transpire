# TProxy must not be abstract

## Issue

This analyzer is tripped if you pass a type to the `TProxy` parameter for `DispatchProxy` that is `abstract`:

```
public abstract class AbstractDispatchProxy
  : DispatchProxy
{
  public AbstractDispatchProxy() { }

  // Invoke() definition goes here...
}

public interface IUsingDispatchProxy { }

var proxy = DispatchProxy.Create<IUsingDispatchProxy, AbstractDispatchProxy>();
```

`DispatchProxy` will only work with proxy types that are not `abstract`, but this won't be an exception until runtime.

## Code Fix

No code fix is available.