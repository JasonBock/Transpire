Rather than try to maintain a list of all members (probably can just focus on methods to start) that return an object of the same type. For example:

```c#
using System.Collections.Immutable;

ImmutableList<string> items = ["a", "b", "c"];
items.Add("3");
```

`Add()` returns an `ImmutableList<string>` value. Same with `AddRange()`. So, for any method that exists on the immutable type, if it returns something and it's of the same type (as a closed generic), and it's not assigned, then flag it.

Of course, we need to determine if it's being "assigned". This:

```c#
items.AddRange(["11", "22"]);
```

Is an `InvocationExpressionSyntax` that's **not** within an `AssignmentExpressionSyntax`. In this case:

```c#
items = items.AddRange(["11", "22"]);
```

This is an `AssignmentExpressionSyntax`. So is the assignment to the `Content` property:

```c#
public class Holder
{
  public required ImmutableList<string> Content { get; set; }
}

var holder = new Holder { Content = [] };

ImmutableList<string> items = ["a", "b", "c"];
holder.Content = items.AddRange(["11", "22"]);
```

So if we do an `Ancestors()` call and look for `AssignmentExpressionSyntax` and we don't find one, then that's an issue.

However...

```c#
items.FindAll(value => ...);
```

`FindAll()` returns an `ImmutableList<>`, so my approach would catch it, but for a different reason. If a user wants to ignore a return value, that's what a [discard](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/discards) is for. There are three style rules (IDE0058, IDE0059, and IDE0060) that can inform the developer that a return value is being ignored/unused. `FindAll()` isn't a "mutation" operation like `AddRange()` and `Remove()`.

Now, `ImmutableList<>` implements `IImmutableList<>`, and the methods on `IImmutableList` are all mutation operations. There are a core set of interfaces:

* DONE - `ImmutableArray<>` - struct
* DONE - `ImmutableDictionary<,>` - class
* DONE - `ImmutableHashSet<>` - class
* DONE - `ImmutableList<>` - class
* DONE - `ImmutableQueue<>` - class
* DONE - `ImmutableSortedSet<>` - class
* DONE - `ImmutableStack<>` - class

There is also [`CA1806`](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1806). If a method is marked with `[Pure]`, that would trip `CA1806`. Unfortunately, this would require work done with the immutable types, and that would be a non-trivial "lift". Funny, it seems like they were there, but they were removed: https://github.com/dotnet/runtime/pull/35118. Related: https://github.com/dotnet/runtime/issues/34098.

So here's the order of operations:

* Look for method invocations that are instance (not static).
* Look for the containing type such that it's one of the immutable target types in the above list.
* Look to see if there's a return value, and if there is, does it match the containing type.
* If all is true, see if there's an ancestor that is of type `AssignmentExpressionSyntax`