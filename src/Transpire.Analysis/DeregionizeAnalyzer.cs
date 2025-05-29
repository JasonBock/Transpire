using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds <c>region</c> and <c>endregion</c> usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeregionizeAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DeregionizeDescriptor.Create();

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

		context.RegisterSyntaxTreeAction(nodeAction =>
		{
			if (nodeAction.Tree.HasRegionDirectives(nodeAction.CancellationToken))
			{
				nodeAction.ReportDiagnostic(Diagnostic.Create(
					DeregionizeAnalyzer.rule, 
					nodeAction.Tree.GetRoot(nodeAction.CancellationToken).GetLocation()));
			}
		});
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DeregionizeAnalyzer.rule];
}