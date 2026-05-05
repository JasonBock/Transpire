# No Return Value Assignment With Immutable Collections

## Issue

Immutable collections have methods like `Add()`, `Enqueue()`, etc. that return a new immutable collection that contains the changes. Developers must not ignore these return values, otherwise they may end up with incorrect code:

```c#
public static int GetItemCount()
{
  ImmutableList<int> items = [2, 3, 4];
	items.Add(20);
  return items.Count
}
```

In this code, 3 will be returned, where the developer may think that an item has been added and it should return 4.

## Code Fix

There is no code fix available for this analyzer.