# Consider defining this method with 4 or less generic parameters

## Issue

This analyzer is tripped depending on how many parameters you have defined for a method:

* If the parameter count is over 32, this will create an error diagnostic (the maximum is 65535, read [this article](https://www.tabsoverspaces.com/233802-whats-the-maximum-number-of-generic-parameters-for-a-class-in-net-csharp) for more information).
* If the parameter count is over 16, this will create a warning diagnostic.
* If the parameter count is over 4, this will create an informational diagnostic.

## Code Fix

No code fix is available.