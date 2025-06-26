What to search for:

* `LiteralExpressionSyntax` - Kind is `NumericLiteralExpression`

What can it be contained in?
* `LocalDeclarationStatementSyntax` - derives from `StatementSyntax`
* `MemberDeclarationSyntax` - like `ClassDeclarationSyntax` (I think)
* `BlockSyntax`
* `ArgumentListSyntax`

Things can get very interesting in terms of formatting, like `var x = -0XFFD33L;`. More info:
* https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
* `Uri.IsHexDigit()` - https://learn.microsoft.com/en-us/dotnet/api/system.uri.ishexdigit?view=net-9.0

Create a read-only, deconstructable `LiteralNumberInformation` class that takes the `string` of `LiteralExpressionSyntax`'s token value. It will have properties like:

* `NegativeSign` - either `"-"` or `""` 
* `Prefix` - either `"0x/X"` or `"0b/B"` or `""`
* `WholePart` - the "integer" part. For "123", this would be `"123"`. "123.45" would give `"123"`.
* `DecimalPoint` - either `"."` or `""`
* `FractionalPart` - the "remainder" part. For "123", this would be `""`. "123.45" would give `"45"`.
* `Exponent` - for floating points, something like `"e2"`
* `TypeSuffix` - can be `"U"` or `"UL"` or `"d"` or `""`
* `NeedsSeparators` - returns `true` if `WholePart` or `FractionalPart` should have a separator in them.

This should make it easier to create a separated version of the literal. `LiteralNumberInformation CreateSeparated(uint spacingSize)`. If it's already separated, it just returns `this`. Otherwise, an new one.

`ToString()` combines all the parts together.

Also, it's possible that a declaration like `0b100011211` will end up having two `LiteralExpressionSyntax` values. This may confuse the analyzer

Floor((Length - 1) / spacingSize)

4, 3 => 1
5, 3 => 1
6, 3 => 1
7, 3 => 2
8, 3 => 2
9, 3 => 2
10, 3 => 3
11, 3 => 3
12, 3 => 3
13, 3 => 4

5, 4 => 1
6, 4 => 1
7, 4 => 1
8, 4 => 1
9, 4 => 2
10, 4 => 2
11, 4 => 2
12, 4 => 2
13, 4 => 3

This should work.