# Do not create a new DateTime via its no-argument constructor

## Issue

This analyzer is tripped if you create a `DateTime` instance by calling its' no-argument constructor:

```
var now = new DateTime();
```

This returns a `DateTime` value that is in the current time zone. This can be problematic if time zone conversions need to occur.

## Code Fix

A code fix is available, which will suggest changing the constructor call to `DateTime.UtcNow`.