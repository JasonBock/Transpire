using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that looks for <c>TryParse</c> calls and recommends
/// <c>Parse</c> when applicable.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RecommendTryParseOverParseAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = RecommendTryParseOverParseDescriptor.Create();

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
			compilationContext.RegisterOperationAction(operationContext =>
			{
				RecommendTryParseOverParseAnalyzer.AnalyzeOperationAction(operationContext);
			}, OperationKind.Invocation);
		});
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context)
	{
		var invocationOperation = (IInvocationOperation)context.Operation;
		var invocationReference = invocationOperation.TargetMethod;

		if (invocationReference.Name == "Parse" &&
			invocationReference.IsStatic &&
			invocationReference.Parameters.Length == 1 &&
			invocationReference.Parameters[0].Type.SpecialType == SpecialType.System_String &&
			SymbolEqualityComparer.Default.Equals(invocationReference.ReturnType, invocationReference.ContainingType))
		{
			var invocationContainingType = invocationReference.ContainingType;

			var hasTryParse = (from member in invocationContainingType.GetMembers("TryParse")
								where member.Kind == SymbolKind.Method
								let method = member as IMethodSymbol
								where method.IsStatic &&
								method.Parameters.Length == 2 &&
								method.Parameters[0].Type.SpecialType == SpecialType.System_String &&
								method.Parameters[1].RefKind == RefKind.Out &&
								SymbolEqualityComparer.Default.Equals(
									method.Parameters[1].Type, invocationReference.ContainingType) &&
								!method.ReturnsVoid &&
								method.ReturnType.SpecialType == SpecialType.System_Boolean
								select method).Any();

			if (hasTryParse)
			{
				var properties = ImmutableDictionary.CreateBuilder<string, string?>();
				properties.Add(Constants.RecommendTryParseOverParseParameterTypeName,
					invocationReference.ContainingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
				properties.Add(Constants.RecommendTryParseOverParseParameterValue,
					invocationOperation.Arguments[0].Syntax.GetText().ToString());

				context.ReportDiagnostic(Diagnostic.Create(RecommendTryParseOverParseAnalyzer.rule,
					context.Operation.Syntax.GetLocation(),
					properties: properties.ToImmutable()));
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[RecommendTryParseOverParseAnalyzer.rule];
}