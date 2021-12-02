using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindNewGuidViaConstructorAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		FindNewGuidViaConstructorDescriptor.Create();

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
			var guidSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(Guid).FullName);

			if (guidSymbol is not null)
			{
				var guidConstructorSymbol = guidSymbol.InstanceConstructors.SingleOrDefault(_ => _.Parameters.Length == 0);

				if (guidConstructorSymbol is not null)
				{
					compilationContext.RegisterOperationAction(operationContext =>
						{
						  FindNewGuidViaConstructorAnalyzer.AnalyzeOperationAction(
									operationContext, guidConstructorSymbol);
					  }, OperationKind.ObjectCreation);
				}
			}
		});
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context, IMethodSymbol guidConstructorSymbol)
	{
		var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

		if (SymbolEqualityComparer.Default.Equals(contextInvocation, guidConstructorSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				FindNewGuidViaConstructorAnalyzer.rule, context.Operation.Syntax.GetLocation()));
		}
	}

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		ImmutableArray.Create(FindNewGuidViaConstructorAnalyzer.rule);
}