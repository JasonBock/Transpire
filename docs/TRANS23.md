# Can Not Use `[Excluded]` and `[Ordered]` On a Property

## Issue

It does not make sense to order a property that is also excluded - this is an error:

```c#
[Equality]
public partial record Customer(
  Guid Id, [property: Excluded, Ordered(3u)] string Name, uint Age);
```

## Code Fix

No code fix is available.