using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds <see langword="goto" /> usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DetectGotoUsageAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DetectGotoUsageDescriptor.Create();

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
				DetectGotoUsageAnalyzer.AnalyzeNode, SyntaxKind.GotoCaseStatement);
			compilationContext.RegisterSyntaxNodeAction(
				DetectGotoUsageAnalyzer.AnalyzeNode, SyntaxKind.GotoDefaultStatement);
			compilationContext.RegisterSyntaxNodeAction(
				DetectGotoUsageAnalyzer.AnalyzeNode, SyntaxKind.GotoStatement);
		});
	}

	private static void AnalyzeNode(SyntaxNodeAnalysisContext context) =>
		context.ReportDiagnostic(
			Diagnostic.Create(DetectGotoUsageDescriptor.Create(),
				context.Node.GetLocation()));

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DetectGotoUsageAnalyzer.rule];
}