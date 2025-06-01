using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds object creation using types from
/// <see cref="System.Collections"/>, such as <see cref="ArrayList"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DiscourageNonGenericCollectionCreationAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DiscourageNonGenericCollectionCreationDescriptor.Create();

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

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationContext =>
		{
			var collectionTypes = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default)
			{
				compilationContext.Compilation.GetTypeByMetadataName(typeof(ArrayList).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Hashtable).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Queue).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(SortedList).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Stack).FullName)!,
			};

			compilationContext.RegisterOperationAction(operationContext =>
			{
				DiscourageNonGenericCollectionCreationAnalyzer.AnalyzeOperationAction(
					operationContext, collectionTypes);
			}, OperationKind.ObjectCreation);
		});
	}

	private static void AnalyzeOperationAction(
		OperationAnalysisContext context, HashSet<INamedTypeSymbol> collectionTypes)
	{
		var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

		if (contextInvocation is not null &&
			collectionTypes.Contains(contextInvocation.ContainingType))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				DiscourageNonGenericCollectionCreationAnalyzer.rule, context.Operation.Syntax.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DiscourageNonGenericCollectionCreationAnalyzer.rule];
}