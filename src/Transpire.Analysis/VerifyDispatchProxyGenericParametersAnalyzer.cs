using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that validates generic parameters on <c>DispatchProxy</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class VerifyDispatchProxyGenericParametersAnalyzer
	: DiagnosticAnalyzer
{
	private const string DispatchProxyTypeFullName = "System.Reflection.DispatchProxy";
	private const string DispatchProxyCreateName = "Create";

	private static readonly DiagnosticDescriptor tIsInterfaceRule =
		VerifyDispatchProxyTIsInterfaceDescriptor.Create();
	private static readonly DiagnosticDescriptor tProxyIsNotAbstractRule =
		VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Create();
	private static readonly DiagnosticDescriptor tProxyIsNotSealedRule =
		VerifyDispatchProxyTProxyIsNotSealedDescriptor.Create();
	private static readonly DiagnosticDescriptor tProxyHasCtorRule =
		VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Create();

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
			var dispatchProxySymbol = compilationContext.Compilation.GetTypeByMetadataName(
				VerifyDispatchProxyGenericParametersAnalyzer.DispatchProxyTypeFullName);

			if (dispatchProxySymbol is null)
			{
				return;
			}

			compilationContext.RegisterOperationAction(operationContext =>
			{
				var createSymbol = (IMethodSymbol)dispatchProxySymbol.GetMembers(
					VerifyDispatchProxyGenericParametersAnalyzer.DispatchProxyCreateName)[0];

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

			if (tType.TypeKind != TypeKind.Interface)
			{
				context.ReportDiagnostic(Diagnostic.Create(VerifyDispatchProxyGenericParametersAnalyzer.tIsInterfaceRule,
					context.Operation.Syntax.GetLocation()));
			}

			var tProxyType = targetMethod.TypeArguments[1];

			if (tProxyType.IsAbstract)
			{
				context.ReportDiagnostic(Diagnostic.Create(VerifyDispatchProxyGenericParametersAnalyzer.tProxyIsNotAbstractRule,
					context.Operation.Syntax.GetLocation()));
			}
			else if (tProxyType.IsSealed)
			{
				context.ReportDiagnostic(Diagnostic.Create(VerifyDispatchProxyGenericParametersAnalyzer.tProxyIsNotSealedRule,
					context.Operation.Syntax.GetLocation()));
			}

			if (!tProxyType.GetMembers()
				.Any(_ => _.Kind == SymbolKind.Method && _.DeclaredAccessibility == Accessibility.Public &&
					!_.IsStatic && _.Name == ".ctor" &&
					((IMethodSymbol)_).Parameters.Length == 0))
			{
				context.ReportDiagnostic(Diagnostic.Create(VerifyDispatchProxyGenericParametersAnalyzer.tProxyHasCtorRule,
					context.Operation.Syntax.GetLocation()));
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
		   VerifyDispatchProxyGenericParametersAnalyzer.tIsInterfaceRule,
		   VerifyDispatchProxyGenericParametersAnalyzer.tProxyIsNotAbstractRule,
		   VerifyDispatchProxyGenericParametersAnalyzer.tProxyIsNotSealedRule,
		   VerifyDispatchProxyGenericParametersAnalyzer.tProxyHasCtorRule,
		];
}