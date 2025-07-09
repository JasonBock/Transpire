# Consider defining this method with 16 or less parameters

## Issue

This analyzer is tripped depending on how many parameters you have defined for a method:

* If the parameter count is over 8192, this will create an error diagnostic (if you're curious why this number is used, read [this article](https://www.tabsoverspaces.com/233892-whats-the-maximum-number-of-arguments-for-method-in-csharp-and-in-net)).
* If the parameter count is over 16, this will create a warning diagnostic.
* If the parameter count is over 4, this will create an informational diagnostic.

## Code Fix

No code fix is available.