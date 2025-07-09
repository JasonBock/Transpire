# TProxy must not be sealed

## Issue

This analyzer is tripped if you pass a type to the `TProxy` parameter for `DispatchProxy` that is `sealed`:

```
public sealed class SealedDispatchProxy
  : DispatchProxy
{
  // Invoke() definition goes here...
}

public interface IUsingDispatchProxy { }

var proxy = DispatchProxy.Create<IUsingDispatchProxy, SealedDispatchProxy>();
```

`DispatchProxy` will only work with proxy types that aren't `sealed`, but this won't be an exception until runtime.

## Code Fix

No code fix is available.