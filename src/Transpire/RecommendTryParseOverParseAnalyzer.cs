using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class RecommendTryParseOverParseAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = RecommendTryParseOverParseDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(RecommendTryParseOverParseAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(compilationContext =>
			{
				compilationContext.RegisterOperationAction(operationContext =>
				{
					RecommendTryParseOverParseAnalyzer.AnalyzeOperationAction(operationContext);
				}, OperationKind.Invocation);
			});
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context)
		{
			var invocationReference = ((IInvocationOperation)context.Operation).TargetMethod;

			if (invocationReference.Name == "Parse" &&
				invocationReference.IsStatic &&
				SymbolEqualityComparer.Default.Equals(invocationReference.ReturnType, invocationReference.ContainingType))
			{
				context.ReportDiagnostic(Diagnostic.Create(RecommendTryParseOverParseAnalyzer.rule,
					context.Operation.Syntax.GetLocation()));
			}
		}
	}
}