using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using Transpire.Descriptors;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindDateTimeNowAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = FindDateTimeNowDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
			ImmutableArray.Create(FindDateTimeNowAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(compilationContext =>
			{
				var dateTimeNowSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName)!
					.GetMembers(nameof(DateTime.Now)).OfType<IPropertySymbol>().SingleOrDefault();

				if(dateTimeNowSymbol is not null)
				{
					compilationContext.RegisterOperationAction(operationContext =>
					{
						FindDateTimeNowAnalyzer.AnalyzeOperationAction(
							operationContext, dateTimeNowSymbol);
					}, OperationKind.PropertyReference);
				}
			});
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context, IPropertySymbol dateTimeNowSymbol)
		{
			var contextReference = ((IPropertyReferenceOperation)context.Operation).Property;

			if(SymbolEqualityComparer.Default.Equals(contextReference, dateTimeNowSymbol))
			{
				context.ReportDiagnostic(Diagnostic.Create(FindDateTimeNowAnalyzer.rule,
					context.Operation.Syntax.GetLocation()));
			}
		}
	}
}