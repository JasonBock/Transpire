# This string is not an interpolated string

## Issue

This analyzer is tripped if you have a string defined as an interpolated string, but no interpolation occurs:

```
var data = $"My data";
```

This isn't a performance issue - in fact, C# will compile this into a "normal" string. It's just unnecessary to have `$` in the definition.

## Code Fix

A code fix is available, which will suggest removing `$`.