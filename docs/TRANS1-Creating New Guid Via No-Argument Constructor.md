# Do not create a new Guid via its no-argument constructor

## Issue

This analyzer is tripped if you call the no-argument constructor to `Guid`:

```
var id = new Guid();
```

This returns an empty `Guid`. While this is correct behavior, developers typically want to create a new `Guid` value.

## Code Fix

There are three code fixes for this analyzer. They will change the call to either `Guid.NewGuid()`, `Guid.Empty`, or `default(Guid)`.