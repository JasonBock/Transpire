# Can Only Use `[Equality]` on Records

## Issue

The `[Equality]` attribute can only be on `record` types. Using it on a `class`, `struct`, or an `interface` is an error:

```c#
[Equality]
public partial class Customer
{
  public Guid Id { get; init; }
  [property: Excluded] public string? Name { get; init; }
}
```

## Code Fix

No code fix is available.