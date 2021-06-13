using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using System;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindNewGuidViaConstructorAnalyzer
		: DiagnosticAnalyzer
	{
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(CallingNewGuidDescriptor.Create());

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			context.RegisterOperationAction(FindNewGuidViaConstructorAnalyzer.AnalyzeConstructorInvocation, OperationKind.Invocation);
		}

		private static void AnalyzeConstructorInvocation(OperationAnalysisContext obj) => throw new NotImplementedException();
	}
}