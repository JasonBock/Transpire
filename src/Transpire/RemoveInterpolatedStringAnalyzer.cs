using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RemoveInterpolatedStringAnalyzer
	: DiagnosticAnalyzer
{
	private static readonly DiagnosticDescriptor rule = RemoveInterpolatedStringDescriptor.Create();

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		ImmutableArray.Create(RemoveInterpolatedStringAnalyzer.rule);

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
				RemoveInterpolatedStringAnalyzer.AnalyzeInterpolatedStringSyntax, SyntaxKind.InterpolatedStringExpression));
	}

	private static void AnalyzeInterpolatedStringSyntax(SyntaxNodeAnalysisContext context)
	{
		var interpolatedStringNode = (InterpolatedStringExpressionSyntax)context.Node;

		if (!interpolatedStringNode.Contents.Any(_ => _.Kind() == SyntaxKind.Interpolation))
		{
			context.ReportDiagnostic(Diagnostic.Create(RemoveInterpolatedStringAnalyzer.rule,
				interpolatedStringNode.GetLocation()));
		}
	}
}