using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Analyzers;

/// <summary>
/// An analyzer that finds <c>== null</c> or <c>!= null</c> usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindNullChecksWithOperatorsAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		FindNullChecksWithOperatorsDescriptor.Create();

	/// <summary>
	/// Initializes the analyzer.
	/// </summary>
	/// <param name="context">An <see cref="AnalysisContext"/> instance.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationContext =>
		{
			compilationContext.RegisterSyntaxNodeAction(
				FindNullChecksWithOperatorsAnalyzer.AnalyzeNode, SyntaxKind.EqualsExpression);
			compilationContext.RegisterSyntaxNodeAction(
				FindNullChecksWithOperatorsAnalyzer.AnalyzeNode, SyntaxKind.NotEqualsExpression);
		});
	}

	private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
	{
		var node = (BinaryExpressionSyntax)context.Node;

		if (node.Left.Kind() == SyntaxKind.NullLiteralExpression ||
			node.Right.Kind() == SyntaxKind.NullLiteralExpression)
		{
			if (!FindNullChecksWithOperatorsAnalyzer.IsInsideExpressionTree(node, context.SemanticModel))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(FindNullChecksWithOperatorsDescriptor.Create(),
						context.Node.GetLocation()));
			}
		}
	}

	static bool IsInsideExpressionTree(SyntaxNode node, SemanticModel model)
	{
		// Walk up to the nearest lambda or anonymous method
		var lambda = node.AncestorsAndSelf()
							  .FirstOrDefault(n => n is LambdaExpressionSyntax || n is AnonymousMethodExpressionSyntax);

		if (lambda is null)
		{
			return false;
		}

		// Get the type the lambda is being converted to
		var typeInfo = model.GetTypeInfo(lambda);
		var convertedType = typeInfo.ConvertedType;

		// Check if it's Expression<TDelegate>
		return convertedType is not null &&
			convertedType.OriginalDefinition.DerivesFrom(model.Compilation.GetTypeByMetadataName("System.Linq.Expressions.Expression")!);
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindNullChecksWithOperatorsAnalyzer.rule];
}