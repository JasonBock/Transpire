# All Properties Excluded

## Issue

`[Equality]` exists on a record and all of its' properties are marked `[Excluded]`:

```c#
[Equality]
public partial record Customer(
  [property: Excluded] Guid Id, 
  [property: Excluded] string Name, 
  [property: Excluded] uint Age);
```

## Code Fix

No code fix is available.