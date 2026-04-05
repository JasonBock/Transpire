using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Generators.Models;

namespace Transpire.Analysis.Analyzers;

/// <summary>
/// An analyzer that looks for invalid equality attribute usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EqualityAnalyzer
	: DiagnosticAnalyzer
{
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
			var equalityAttributeSymbol = compilationContext.Compilation.GetTypeByMetadataName(
				Constants.EqualityAttributeName)!;
			var excludedAttributeSymbol = compilationContext.Compilation.GetTypeByMetadataName(
				Constants.ExcludedAttributeName)!;
			var orderedAttributeSymbol = compilationContext.Compilation.GetTypeByMetadataName(
				Constants.OrderedAttributeName)!;

			compilationContext.RegisterOperationAction(operationContext =>
			{
				AnalyzeEqualityAttribute(
					operationContext, equalityAttributeSymbol);
				//AnalyzeDependentAttributes(
				//	operationContext, excludedAttributeSymbol, orderedAttributeSymbol);
			}, OperationKind.Attribute);
		});
	}

	private static void AnalyzeEqualityAttribute(
		OperationAnalysisContext context, INamedTypeSymbol equalityAttributeSymbol)
	{
		if (context.Operation is IAttributeOperation { Operation: IObjectCreationOperation attribute })
		{
			var attributeType = attribute.Constructor?.ContainingType;

			if (attributeType is not null &&
				SymbolEqualityComparer.Default.Equals(attributeType, equalityAttributeSymbol))
			{
				var recordSymbol = context.ContainingSymbol as ITypeSymbol;

				if (recordSymbol is not null)
				{
					var recordModel = RecordModelGenerator.Create(context.Operation.Syntax, recordSymbol, context.Compilation);

					foreach (var diagnostic in recordModel.Diagnostics)
					{
						context.ReportDiagnostic(diagnostic);
					}
				}
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
			AllPropertiesExcludedDescriptor.Create(),
			CannotUseExcludedAndOrderedOnPropertyDescriptor.Create(),
			CanOnlyUseEqualityAttributeOnRecordsDescriptor.Create(),
			NoExcludedOrOrderedUsageDescriptor.Create(),
			OnePropertyOrderedDescriptor.Create(),
		];
}