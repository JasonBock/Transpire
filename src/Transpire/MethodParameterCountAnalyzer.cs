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
	// TODO: Just provide the default values
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
		   MethodParameterCountInfoDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultInfoLimit),
		   MethodParameterCountWarningDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultWarningLimit),
		   MethodParameterCountErrorDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultErrorLimit),
		];

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
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var configuration = new MethodParameterCountAnalyzerConfiguration(options);
		var method = (MethodDeclarationSyntax)context.Node;

		if (method.ParameterList.Parameters.Count > configuration.ErrorLimit)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountErrorDescriptor.Create(configuration.ErrorLimit.Value),
				method.Identifier.GetLocation()));
		}
		else if (method.ParameterList.Parameters.Count > configuration.WarningLimit)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountWarningDescriptor.Create(configuration.WarningLimit.Value),
				method.Identifier.GetLocation()));
		}
		else if (method.ParameterList.Parameters.Count > configuration.InfoLimit)
		{
			context.ReportDiagnostic(Diagnostic.Create(MethodParameterCountInfoDescriptor.Create(configuration.InfoLimit.Value),
				method.Identifier.GetLocation()));
		}
	}
}