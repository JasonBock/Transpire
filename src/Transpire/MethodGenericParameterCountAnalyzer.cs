using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MethodGenericParameterCountAnalyzer
	: DiagnosticAnalyzer
{
	// TODO: Just provide the default values
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		ImmutableArray.Create(
			MethodGenericParameterCountInfoDescriptor.Create(MethodGenericParameterCountAnalyzerConfiguration.DefaultInfoLimit),
			MethodGenericParameterCountWarningDescriptor.Create(MethodGenericParameterCountAnalyzerConfiguration.DefaultWarningLimit),
			MethodGenericParameterCountErrorDescriptor.Create(MethodGenericParameterCountAnalyzerConfiguration.DefaultErrorLimit));

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
				MethodGenericParameterCountAnalyzer.AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration));
	}

	private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
	{
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var configuration = new MethodGenericParameterCountAnalyzerConfiguration(options);
		var method = (MethodDeclarationSyntax)context.Node;

		if (method.TypeParameterList is not null)
		{
			if (method.TypeParameterList.Parameters.Count > configuration.ErrorLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountErrorDescriptor.Create(configuration.ErrorLimit.Value),
					method.Identifier.GetLocation()));
			}
			else if (method.TypeParameterList.Parameters.Count > configuration.WarningLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountWarningDescriptor.Create(configuration.WarningLimit.Value),
					method.Identifier.GetLocation()));
			}
			else if (method.TypeParameterList.Parameters.Count > configuration.InfoLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountInfoDescriptor.Create(configuration.InfoLimit.Value),
					method.Identifier.GetLocation()));
			}
		}
	}
}