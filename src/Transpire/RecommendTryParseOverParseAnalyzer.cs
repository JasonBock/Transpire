using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using System.Linq;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class RecommendTryParseOverParseAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = RecommendTryParseOverParseDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(RecommendTryParseOverParseAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
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
					properties.Add(RecommendTryParseOverParseDescriptor.ParameterTypeName,
						invocationReference.ContainingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
					properties.Add(RecommendTryParseOverParseDescriptor.ParameterValue,
						invocationOperation.Arguments[0].Syntax.GetText().ToString());

					context.ReportDiagnostic(Diagnostic.Create(RecommendTryParseOverParseAnalyzer.rule,
						context.Operation.Syntax.GetLocation(),
						properties: properties.ToImmutable()));
				}
			}
		}
	}
}