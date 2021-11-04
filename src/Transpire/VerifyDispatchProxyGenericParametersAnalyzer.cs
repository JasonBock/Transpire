using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Reflection;
using Transpire.Descriptors;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class VerifyDispatchProxyGenericParametersAnalyzer
		: DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor tIsInterfaceRule =
			VerifyDispatchProxyTIsInterfaceDescriptor.Create();
		private static readonly DiagnosticDescriptor tProxyIsNotSealedRule =
			VerifyDispatchProxyTProxyIsNotSealedDescriptor.Create();
		private static readonly DiagnosticDescriptor tProxyHasCtorRule =
			VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Create();

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(compilationContext =>
			{
				var dispatchProxySymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DispatchProxy).FullName);

				if (dispatchProxySymbol is null)
				{
					return;
				}

				compilationContext.RegisterOperationAction(operationContext =>
				{
					var createSymbol = (IMethodSymbol)dispatchProxySymbol.GetMembers(nameof(DispatchProxy.Create))[0];

					VerifyDispatchProxyGenericParametersAnalyzer.AnalyzeOperationAction(
						operationContext, createSymbol);
				}, OperationKind.Invocation);
			});
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context,
			IMethodSymbol createSymbol)
		{
			var targetMethod = ((IInvocationOperation)context.Operation).TargetMethod;

			if (SymbolEqualityComparer.Default.Equals(targetMethod.ConstructedFrom, createSymbol))
			{
				var tType = targetMethod.TypeArguments[0];

				if(tType.TypeKind != TypeKind.Interface)
				{
					context.ReportDiagnostic(Diagnostic.Create(VerifyDispatchProxyGenericParametersAnalyzer.tIsInterfaceRule,
						context.Operation.Syntax.GetLocation()));
				}
			}
		}

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(
				VerifyDispatchProxyGenericParametersAnalyzer.tIsInterfaceRule,
				VerifyDispatchProxyGenericParametersAnalyzer.tProxyIsNotSealedRule,
				VerifyDispatchProxyGenericParametersAnalyzer.tProxyHasCtorRule);
	}
}