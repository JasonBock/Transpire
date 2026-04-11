# No `[Excluded]` or `[Ordered]` Usage

## Issue

`[Equality]` exists on a record but none of its' properties have `[Excluded]` or `[Ordered]`:

```c#
[Equality]
public partial record Customer(
  Guid Id, string Name, uint Age);
```

## Code Fix

No code fix is available.