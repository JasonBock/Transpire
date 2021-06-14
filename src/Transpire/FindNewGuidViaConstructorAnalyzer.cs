using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.Operations;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindNewGuidViaConstructorAnalyzer
		: DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor rule = CallingNewGuidDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(FindNewGuidViaConstructorAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			//context.RegisterSymbolAction(FindNewGuidViaConstructorAnalyzer.AnalyzeSymbolInvocation, SymbolKind.Method);
			context.RegisterOperationAction(FindNewGuidViaConstructorAnalyzer.AnalyzeOperationAction, OperationKind.ObjectCreation);
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context)
		{
			var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;
			var guidSymbol = context.Compilation.GetTypeByMetadataName(typeof(Guid).FullName)!
				.InstanceConstructors.Single(_ => _.Parameters.Length == 0);

			if(SymbolEqualityComparer.Default.Equals(contextInvocation, guidSymbol))
			{
				var diagnostic = Diagnostic.Create(FindNewGuidViaConstructorAnalyzer.rule, context.ContainingSymbol.Locations[0]);
				context.ReportDiagnostic(diagnostic);
			}
		}

		private static void AnalyzeSymbolInvocation(SymbolAnalysisContext context) 
		{
			var guidSymbol = context.Compilation.GetTypeByMetadataName(typeof(Guid).FullName);
		}
	}
}