using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using Microsoft.CodeAnalysis.CSharp;
using Transpire.Extensions;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DeregionizeAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DeregionizeDescriptor.Create();

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

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DeregionizeAnalyzer.rule];
}