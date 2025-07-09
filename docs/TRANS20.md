# Use `IsNullOrWhiteSpace()` instead of `IsNullOrEmpty()`

## Issue

There are more characters that are considered "white space" characters that `IsNullOrEmpty()` will not detect. It's preferrable to use `IsNullOrWhiteSpace()`.

## Code Fix

A code fix is available to change the invocation from `IsNullOrEmpty()` to `IsNullOrWhiteSpace()`.