Cases:
* `BinaryExpression` is "within" an `ArgumentSyntax` where that argument is of type `Expression<>`
* `BinaryExpression` is "within" a `VariableDeclarationSyntax` whose type is `Expression<>`

I think there's a general way to determine if the node is in an expression tree:

```c#
using System;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{
    static void Main()
    {
        var code = @"
using System;
using System.Linq.Expressions;

class Test {
    void M() {
        Expression<Func<int, int>> expr = x => x + 1;
        Func<int, int> func = x => x + 1;
    }
}";

        var tree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("Demo",
            new[] { tree },
            new[] {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Expression).Assembly.Location)
            });

        var model = compilation.GetSemanticModel(tree);

        // Find all binary expressions
        var root = tree.GetRoot();
        foreach (var binary in root.DescendantNodes().OfType<BinaryExpressionSyntax>())
        {
            bool insideExpressionTree = IsInsideExpressionTree(binary, model);
            Console.WriteLine($"{binary} -> Inside Expression Tree? {insideExpressionTree}");
        }
    }

    static bool IsInsideExpressionTree(SyntaxNode node, SemanticModel model)
    {
        // Walk up to the nearest lambda or anonymous method
        var lambda = node.AncestorsAndSelf()
                         .FirstOrDefault(n => n is LambdaExpressionSyntax || n is AnonymousMethodExpressionSyntax);

        if (lambda == null)
            return false;

        // Get the type the lambda is being converted to
        var typeInfo = model.GetTypeInfo(lambda);
        var convertedType = typeInfo.ConvertedType;

        // Check if it's Expression<TDelegate>
        return convertedType != null &&
               convertedType.OriginalDefinition.ToDisplayString() == "System.Linq.Expressions.Expression<TDelegate>";
    }
}
```

* DONE - Handle TODO in `DeregionizeCodeFix`
* DONE - Make help Markdown doc for this analyzer
* Why aren't the code fixes running in Transpire.Scenarios?

TODO:
* Consider creating an analyzer to discourage usage of `Expression<>`