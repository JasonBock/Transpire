# Do not use DateTimeKind.Local or DateTimeKind.Unspecified in a DateTime constructor

## Issue

This analyzer is tripped if you create a `DateTime` instance using either `DateTimeKind.Local` or `DateTimeKind.Unspecified`:

```
var time = new DateTime(100, DateTimeKind.Local);
```

This returns a `DateTime` value that is in the current time zone. This can be problematic if time zone conversions need to occur.

## Code Fix

A code fix is available, which will suggest changing the `DateTimeKind` value to `DateTimeKind.Utc`.