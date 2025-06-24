using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds numbers with no separators.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DetectNonSeparatedDigitsAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DetectNonSeparatedDigitsDescriptor.Create();

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
			compilationContext.RegisterSyntaxNodeAction(
				DetectNonSeparatedDigitsAnalyzer.AnalyzeLiteralExpression, SyntaxKind.NumericLiteralExpression));
	}

	private static void AnalyzeLiteralExpression(SyntaxNodeAnalysisContext context)
	{
		if (!context.FilterTree.GetRoot().ContainsDiagnostics)
		{
			var literal = (LiteralExpressionSyntax)context.Node;
			var literalInformation = new LiteralNumberInformation(literal);

			if (literalInformation.NeedsSeparators)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(DetectNonSeparatedDigitsDescriptor.Create(),
						literal.GetLocation()));
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DetectNonSeparatedDigitsAnalyzer.rule];
}