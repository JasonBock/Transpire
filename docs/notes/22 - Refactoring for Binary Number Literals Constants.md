What to search for:

* `LiteralExpressionSyntax` - Kind is `NumericLiteralExpression`

What can it be contained in?
* `LocalDeclarationStatementSyntax` - derives from `StatementSyntax`
* `MemberDeclarationSyntax` - like `ClassDeclarationSyntax` (I think)

Things can get very interesting in terms of formatting, like `var x = -0XFFD33L;`. More info:
* https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
* `Uri.IsHexDigit()` - https://learn.microsoft.com/en-us/dotnet/api/system.uri.ishexdigit?view=net-9.0