# Use TryParse() instead of Parse()

## Issue

This analyzer is tripped if you use a `Parse()` method rather than `TryParse()`:

```
var id = int.Parse("3");
```

This can raise an exception if the parse is not successful.

## Code Fix

A code fix is available, which will suggest changing the `Parse()` call to `TryParse()`.