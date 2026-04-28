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