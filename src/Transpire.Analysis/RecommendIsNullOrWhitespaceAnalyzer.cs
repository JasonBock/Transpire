using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that looks for <see cref="string.IsNullOrEmpty(string)"/> calls and recommends
/// <see cref="string.IsNullOrWhiteSpace(string)"/> when applicable.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RecommendIsNullOrWhiteSpaceAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = RecommendIsNullOrWhiteSpaceDescriptor.Create();

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
				RecommendIsNullOrWhiteSpaceAnalyzer.AnalyzeOperationAction(operationContext);
			}, OperationKind.Invocation);
		});
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context)
	{
		var invocationOperation = (IInvocationOperation)context.Operation;
		var invocationReference = invocationOperation.TargetMethod;

		if (invocationReference.Name == nameof(string.IsNullOrEmpty) &&
			invocationReference.IsStatic &&
			invocationReference.Parameters.Length == 1 &&
			invocationReference.Parameters[0].Type.SpecialType == SpecialType.System_String &&
			invocationReference.ContainingType.SpecialType == SpecialType.System_String)
		{
			context.ReportDiagnostic(Diagnostic.Create(RecommendIsNullOrWhiteSpaceAnalyzer.rule,
				context.Operation.Syntax.GetLocation()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[RecommendIsNullOrWhiteSpaceAnalyzer.rule];
}