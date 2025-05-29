using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds <see cref="DateTime()" /> usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindNewDateTimeViaConstructorAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		FindNewDateTimeViaConstructorDescriptor.Create();

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

		context.RegisterCompilationStartAction(compilationContext =>
		{
			var dateTimeSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName);

			if (dateTimeSymbol is not null)
			{
				var dateTimeNoArgumentConstructorSymbol = dateTimeSymbol.InstanceConstructors
					.SingleOrDefault(_ => _.Parameters.Length == 0);

				if (dateTimeNoArgumentConstructorSymbol is not null)
				{
					compilationContext.RegisterOperationAction(operationContext =>
					{
						FindNewDateTimeViaConstructorAnalyzer.AnalyzeOperationAction(
							operationContext, dateTimeNoArgumentConstructorSymbol);
					}, OperationKind.ObjectCreation);
				}
			}
		});
	}

	private static void AnalyzeOperationAction(
		OperationAnalysisContext context, IMethodSymbol dateTimeNoArgumentConstructorSymbol)
	{
		var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

		if (SymbolEqualityComparer.Default.Equals(contextInvocation, dateTimeNoArgumentConstructorSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				FindNewDateTimeViaConstructorAnalyzer.rule, context.Operation.Syntax.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindNewDateTimeViaConstructorAnalyzer.rule];
}