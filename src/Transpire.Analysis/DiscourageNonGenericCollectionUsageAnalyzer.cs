using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
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

			compilationContext.RegisterOperationAction(operationContext =>
				DiscourageNonGenericCollectionUsageAnalyzer.AnalyzeObjectCreationOperationAction(
					operationContext, collectionTypes),
				OperationKind.ObjectCreation);
			compilationContext.RegisterSyntaxNodeAction(syntaxNodeContext =>
				DiscourageNonGenericCollectionUsageAnalyzer.AnalyzeTypeDeclaration(
					syntaxNodeContext, collectionTypes),
				SyntaxKind.ClassDeclaration);
			compilationContext.RegisterSyntaxNodeAction(syntaxNodeContext =>
				DiscourageNonGenericCollectionUsageAnalyzer.AnalyzeMethodDeclaration(
					syntaxNodeContext, collectionTypes),
				SyntaxKind.MethodDeclaration);
		});
	}

	private static void AnalyzeObjectCreationOperationAction(
		OperationAnalysisContext context, HashSet<ITypeSymbol> collectionTypes)
	{
		var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

		if (contextInvocation is not null &&
			collectionTypes.Contains(contextInvocation.ContainingType))
		{
			context.ReportDiagnostic(Diagnostic.Create(
				DiscourageNonGenericCollectionUsageAnalyzer.rule, context.Operation.Syntax.GetLocation()));
		}
	}

	private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context,
		HashSet<ITypeSymbol> collectionTypes)
	{
		var method = (MethodDeclarationSyntax)context.Node;
		var model = context.SemanticModel;
		var methodSymbol = model.GetDeclaredSymbol(method) as IMethodSymbol;

		if (methodSymbol is not null)
		{
			if (!methodSymbol.ReturnsVoid && collectionTypes.Contains(methodSymbol.ReturnType))
			{
				context.ReportDiagnostic(Diagnostic.Create(
					DiscourageNonGenericCollectionUsageAnalyzer.rule, method.ReturnType.GetLocation()));
			}

			for (var i = 0; i < methodSymbol.Parameters.Length; i++)
			{
				var parameter = methodSymbol.Parameters[i];
				var parameterNodes = method.ParameterList.Parameters;

				if (collectionTypes.Contains(parameter.Type))
				{
					context.ReportDiagnostic(Diagnostic.Create(
						DiscourageNonGenericCollectionUsageAnalyzer.rule, parameterNodes[i].Type!.GetLocation()));
				}
			}
		}
	}

	private static void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context,
		HashSet<ITypeSymbol> collectionTypes)
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
				DiscourageNonGenericCollectionUsageAnalyzer.rule, type.Identifier.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[DiscourageNonGenericCollectionUsageAnalyzer.rule];
}