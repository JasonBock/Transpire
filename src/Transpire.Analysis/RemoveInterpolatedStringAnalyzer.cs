using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that looks for strings starting with <c>$</c>
/// and have no interpolation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveInterpolatedStringAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = RemoveInterpolatedStringDescriptor.Create();

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

		context.ConfigureGeneratedCodeAnalysis(
			GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationContext =>
			compilationContext.RegisterSyntaxNodeAction(
				RemoveInterpolatedStringAnalyzer.AnalyzeInterpolatedStringSyntax, SyntaxKind.InterpolatedStringExpression));
	}

	private static void AnalyzeInterpolatedStringSyntax(SyntaxNodeAnalysisContext context)
	{
		var interpolatedStringNode = (InterpolatedStringExpressionSyntax)context.Node;

		if (!interpolatedStringNode.Contents.Any(_ => _.Kind() == SyntaxKind.Interpolation))
		{
			context.ReportDiagnostic(Diagnostic.Create(RemoveInterpolatedStringAnalyzer.rule,
				interpolatedStringNode.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[RemoveInterpolatedStringAnalyzer.rule];
}