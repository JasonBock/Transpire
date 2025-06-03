using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds type definitions that use non-generic types from
/// <see cref="System.Collections"/>, such as <see cref="ArrayList"/>
/// in its inheritance hierarchy.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule =
		DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Create();

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

			compilationContext.RegisterSyntaxNodeAction(syntaxNodeContext =>
				DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer.AnalyzeTypeDeclaration(syntaxNodeContext, collectionTypes),
				SyntaxKind.ClassDeclaration);
		});
	}

	private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context,
		HashSet<INamedTypeSymbol> collectionTypes)
	{
		var type = (TypeDeclarationSyntax)context.Node;
		var model = context.SemanticModel;
		var typeSymbol = model.GetDeclaredSymbol(type);

		var parentBaseType = typeSymbol?.BaseType;
		var foundNonGenericBaseType = false;

		while (parentBaseType is not null)
		{
			if (collectionTypes.Contains(parentBaseType))
			{
				foundNonGenericBaseType = true;
				break;
			}

			parentBaseType = parentBaseType.BaseType;
		}

		if (foundNonGenericBaseType)
		{
			context.ReportDiagnostic(Diagnostic.Create(
				DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer.rule, type.Identifier.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer.rule];
}