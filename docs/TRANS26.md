# One Property Marked With `[Ordered]`

## Issue

`[Equality]` exists on a record and only one property is used for equality operations, and it has `[Ordered]` - this is an error as it makes no sense to order one property:

```c#
[Equality]
public partial record Customer([property: Ordered(3u)] Guid Id);
```

## Code Fix

No code fix is available.