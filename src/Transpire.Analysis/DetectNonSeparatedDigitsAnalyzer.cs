using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

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

			var literalText = literal.Token.Text;

			if (!literalText.Contains('_'))
			{
				// TODO: I need to be concerned about literal suffixes
				// and negative numbers and exponents, like
				// var x = 0x3345ul;
				// and
				// var x = -0x3345L;
				// and
				// var d = 3_000e2f;
				// and
				// var d = 0x748319789418f; // note this is actually a long
				//
				//var d = 0x74L;

				var reportDiagnostic = false;

				if (literalText.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ||
					literalText.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
				{
					if (literalText.Length > 4)
					{
						reportDiagnostic = true;
					}
				}
				else
				{
					var dotPosition = literalText.IndexOf('.');

					if (dotPosition > -1)
					{
						if (literalText.AsSpan(0, dotPosition).Length > 3 ||
							literalText.AsSpan(dotPosition + 1).Length > 3)
						{
							reportDiagnostic = true;
						}
					}
					else if (literalText.Length > 3)
					{
						reportDiagnostic = true;
					}
				}

				if (reportDiagnostic)
				{
					context.ReportDiagnostic(
						Diagnostic.Create(DetectNonSeparatedDigitsDescriptor.Create(),
							literal.Token.GetLocation()));
				}
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DetectNonSeparatedDigitsAnalyzer.rule];
}