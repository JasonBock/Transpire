# `[Excluded]` or `[Ordered]` Without `[Equality]`

## Issue

`[Excluded]` and/or `[Ordered]` exists on a record that is not marked with `[Equality]` - this is an error as the source generator needs `[Excluded]` to find the other two attributes:

```c#
public partial record Customer(
  Guid Id, [property: Ordered(3u)] string Name, uint Age);
```

## Code Fix

No code fix is available.