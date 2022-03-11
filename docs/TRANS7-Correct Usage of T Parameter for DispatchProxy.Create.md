# T must be an interface

## Issue

This analyzer is tripped if you pass a type to the `T` parameter for `DispatchProxy` that is not an interface:

```
public class CorrectDispatchProxy
  : DispatchProxy
{
  // Invoke() definition goes here...
}

public class UsingDispatchProxy { }

var proxy = DispatchProxy.Create<UsingDispatchProxy, CorrectDispatchProxy>();
```

`DispatchProxy` will only work with an interface, but this won't be an exception until runtime.

## Code Fix

No code fix is available.