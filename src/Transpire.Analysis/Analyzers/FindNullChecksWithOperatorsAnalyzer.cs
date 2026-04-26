using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

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
			context.ReportDiagnostic(
				Diagnostic.Create(FindNullChecksWithOperatorsDescriptor.Create(),
					context.Node.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindNullChecksWithOperatorsAnalyzer.rule];
}