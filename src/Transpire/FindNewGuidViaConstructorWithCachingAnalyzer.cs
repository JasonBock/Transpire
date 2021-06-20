using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Linq;
using Transpire.Descriptors;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindNewGuidViaConstructorWithCachingAnalyzer
		: DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor rule = 
			FindNewGuidViaConstructorDescriptor.Create();
#pragma warning disable RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer
		private static IMethodSymbol? guidConstructorSymbol;
#pragma warning restore RS1008 // Avoid storing per-compilation data into the fields of a diagnostic analyzer

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			context.RegisterOperationAction(
				FindNewGuidViaConstructorWithCachingAnalyzer.AnalyzeOperationAction, OperationKind.ObjectCreation);
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context)
		{
			var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

			if(FindNewGuidViaConstructorWithCachingAnalyzer.guidConstructorSymbol is null)
			{
				FindNewGuidViaConstructorWithCachingAnalyzer.guidConstructorSymbol =
					context.Compilation.GetTypeByMetadataName(typeof(Guid).FullName)!
						.InstanceConstructors.Single(_ => _.Parameters.Length == 0);
			}	

			if(SymbolEqualityComparer.Default.Equals(contextInvocation, FindNewGuidViaConstructorWithCachingAnalyzer.guidConstructorSymbol))
			{
				context.ReportDiagnostic(Diagnostic.Create(
					FindNewGuidViaConstructorWithCachingAnalyzer.rule, context.Operation.Syntax.GetLocation()));
			}
		}

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(FindNewGuidViaConstructorWithCachingAnalyzer.rule);
	}
}