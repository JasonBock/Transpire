# Do not use DateTime.Now

## Issue

This analyzer is tripped if you use `Now` on `DateTime`:

```
var now = DateTime.Now;
```

This returns a `DateTime` value that is in the current time zone. This can be problematic if time zone conversions need to occur.

## Code Fix

A code fix is available, which will suggest changing `Now` to `UtcNow`.