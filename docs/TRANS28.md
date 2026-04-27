# Null Check With Operator

## Issue

Using the equals or not equals operators for null checks use the `==` and `!=` operators. It is possible (though very rare) for this operator implementation to not do the right thing with respect to a null check. Using `is` or `is not` patterns do literal checks against `null`. Therefore, this analyzer will flag null checks that use the operators, rather than the patterns:

```c#
using System;

public sealed class Test
{
  // This will trip the analyzer.
  public bool CheckUsingOperator(Test? value) => value == null;

  // This will not trip the analyzer.
  public bool CheckUsingPattern(Test? value) => value is null;
}
```

## Code Fix

There is a code fix available, which will change the `BinaryExpression` to the corresponding pattern match.