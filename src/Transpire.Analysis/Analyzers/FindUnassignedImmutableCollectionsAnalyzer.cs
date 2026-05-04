using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Analyzers;

/// <summary>
/// An analyzer that finds unassigned immutable collections from return values.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindUnassignedImmutableCollectionsAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = FindUnassignedImmutableCollectionsDescriptor.Create();

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
			var dateTimeNowSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName)!
					 .GetMembers(nameof(DateTime.Now)).OfType<IPropertySymbol>().SingleOrDefault();

			if (dateTimeNowSymbol is not null)
			{
				compilationContext.RegisterOperationAction(
					operationContext => FindUnassignedImmutableCollectionsAnalyzer.AnalyzeOperationAction(operationContext),
					OperationKind.Invocation);
			}
		});
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context)
	{
		var invocationOperation = (IInvocationOperation)context.Operation;
		var invocationReference = invocationOperation.TargetMethod;
		var invocationSyntax = context.Operation.Syntax;

		if (!invocationReference.IsStatic &&
			!invocationReference.ReturnsVoid &&
			FindUnassignedImmutableCollectionsAnalyzer.IsImmutableCollection(invocationReference.ContainingType, context.Compilation) &&
			SymbolEqualityComparer.Default.Equals(invocationReference.ReturnType, invocationReference.ContainingType) &&
			!FindUnassignedImmutableCollectionsAnalyzer.IsReturnValueCaptured(invocationReference, invocationSyntax, context.CancellationToken))
		{
			context.ReportDiagnostic(Diagnostic.Create(FindUnassignedImmutableCollectionsAnalyzer.rule,
				invocationSyntax.GetLocation()));
		}
	}

	private static bool IsReturnValueCaptured(IMethodSymbol method, SyntaxNode node, CancellationToken cancellationToken)
	{
		var assignmentExpressionSyntax = node.Ancestors()
			.FirstOrDefault(node => node is AssignmentExpressionSyntax) as AssignmentExpressionSyntax;

		return assignmentExpressionSyntax is not null;
	}

	private static bool IsImmutableCollection(INamedTypeSymbol type, Compilation compilation)
	{
		var comparer = SymbolEqualityComparer.Default;
		var originalType = type.OriginalDefinition;
		return comparer.Equals(originalType, compilation.GetTypeByMetadataName(typeof(ImmutableArray<>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableDictionary<,>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableHashSet<>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableList<>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableQueue<>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableSortedSet<>).FullName!)) ||
			originalType.DerivesFrom(compilation.GetTypeByMetadataName(typeof(ImmutableStack<>).FullName!));
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindUnassignedImmutableCollectionsAnalyzer.rule];
}