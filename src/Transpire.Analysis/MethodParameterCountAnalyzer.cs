using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that reviews the number of parameters for a method.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MethodParameterCountAnalyzer
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

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
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
		MethodParameterCountAnalyzer.AnalyzeMethodParameters(method, context, configuration);
		MethodParameterCountAnalyzer.AnalyzeMethodGenericParameters(method, context, configuration);
	}

	private static void AnalyzeMethodParameters(
		MethodDeclarationSyntax method, SyntaxNodeAnalysisContext context, 
		MethodParameterCountAnalyzerConfiguration configuration)
	{
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

	private static void AnalyzeMethodGenericParameters(
		MethodDeclarationSyntax method, SyntaxNodeAnalysisContext context,
		MethodParameterCountAnalyzerConfiguration configuration)
	{
		if (method.TypeParameterList is not null)
		{
			if (method.TypeParameterList.Parameters.Count > configuration.GenericErrorLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountErrorDescriptor.Create(configuration.GenericErrorLimit.Value),
					method.Identifier.GetLocation()));
			}
			else if (method.TypeParameterList.Parameters.Count > configuration.GenericWarningLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountWarningDescriptor.Create(configuration.GenericWarningLimit.Value),
					method.Identifier.GetLocation()));
			}
			else if (method.TypeParameterList.Parameters.Count > configuration.GenericInfoLimit)
			{
				context.ReportDiagnostic(Diagnostic.Create(MethodGenericParameterCountInfoDescriptor.Create(configuration.GenericInfoLimit.Value),
					method.Identifier.GetLocation()));
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
			MethodParameterCountInfoDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultInfoLimit),
			MethodParameterCountWarningDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultWarningLimit),
			MethodParameterCountErrorDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultErrorLimit),
			MethodGenericParameterCountInfoDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultGenericInfoLimit),
			MethodGenericParameterCountWarningDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultGenericWarningLimit),
			MethodGenericParameterCountErrorDescriptor.Create(MethodParameterCountAnalyzerConfiguration.DefaultGenericErrorLimit),
		];
}