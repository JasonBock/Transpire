using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds object creation using non-generic types from
/// <see cref="System.Collections"/>, such as <see cref="ArrayList"/>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DiscourageNonGenericCollectionUsageAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DiscourageNonGenericCollectionUsageDescriptor.Create();

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
			var collectionTypes = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default)
			{
				compilationContext.Compilation.GetTypeByMetadataName(typeof(ArrayList).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Hashtable).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Queue).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(SortedList).FullName)!,
				compilationContext.Compilation.GetTypeByMetadataName(typeof(Stack).FullName)!,
			};

			compilationContext.RegisterSyntaxNodeAction(syntaxNodeContext =>
				DiscourageNonGenericCollectionUsageAnalyzer.AnalyzeIdentifierName(
					syntaxNodeContext, collectionTypes),
				SyntaxKind.IdentifierName);
		});
	}

	private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context,
		HashSet<ITypeSymbol> collectionTypes)
	{
		var type = (IdentifierNameSyntax)context.Node;
		var model = context.SemanticModel;
		var typeSymbol = model.GetSymbolInfo(type).Symbol as ITypeSymbol;

		if(typeSymbol is not null && collectionTypes.Contains(typeSymbol))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				DiscourageNonGenericCollectionUsageAnalyzer.rule, type.Identifier.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DiscourageNonGenericCollectionUsageAnalyzer.rule];
}