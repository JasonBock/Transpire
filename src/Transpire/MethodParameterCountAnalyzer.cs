using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MethodParameterCountAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor infoRule = MethodParameterCountInfoDescriptor.Create();
	private static readonly DiagnosticDescriptor warningRule = MethodParameterCountWarningDescriptor.Create();
	private static readonly DiagnosticDescriptor errorRule = MethodParameterCountErrorDescriptor.Create();

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		ImmutableArray.Create(MethodParameterCountAnalyzer.infoRule,
			MethodParameterCountAnalyzer.warningRule, MethodParameterCountAnalyzer.errorRule);

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
			compilationContext.RegisterSyntaxNodeAction(
				MethodParameterCountAnalyzer.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration));
	}

	private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
	{
		var method = (MethodDeclarationSyntax)context.Node;

		if (method.ParameterList.Parameters.Count > 8192)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountAnalyzer.errorRule,
				method.Identifier.GetLocation()));
		}
		else if (method.ParameterList.Parameters.Count > 16)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountAnalyzer.warningRule,
				method.Identifier.GetLocation()));
		}
		else if (method.ParameterList.Parameters.Count > 4)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountAnalyzer.infoRule,
				method.Identifier.GetLocation()));
		}
	}
}