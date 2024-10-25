using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using Microsoft.CodeAnalysis.Operations;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindDateTimeNowAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = FindDateTimeNowDescriptor.Create();

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindDateTimeNowAnalyzer.rule];

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
		{
			var dateTimeNowSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName)!
					 .GetMembers(nameof(DateTime.Now)).OfType<IPropertySymbol>().SingleOrDefault();

			if (dateTimeNowSymbol is not null)
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

		if (SymbolEqualityComparer.Default.Equals(contextReference, dateTimeNowSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(FindDateTimeNowAnalyzer.rule,
				context.Operation.Syntax.GetLocation()));
		}
	}
}