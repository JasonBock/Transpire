using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Descriptors;
using System.Linq;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class RemoveInterpolatedStringAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = RemoveInterpolatedStringDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
			ImmutableArray.Create(RemoveInterpolatedStringAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(compilationContext =>
			{
				compilationContext.RegisterSyntaxNodeAction(
					RemoveInterpolatedStringAnalyzer.AnalyzeInterpolatedStringSyntax, SyntaxKind.InterpolatedStringExpression); 
			});
		}

		private static void AnalyzeInterpolatedStringSyntax(SyntaxNodeAnalysisContext context)
		{
			var stringNode = (InterpolatedStringExpressionSyntax)context.Node;
			
			if(!stringNode.Contents.Any(_ => _.Kind() == SyntaxKind.Interpolation))
			{
				context.ReportDiagnostic(Diagnostic.Create(RemoveInterpolatedStringAnalyzer.rule,
					stringNode.GetLocation()));
			}
		}
	}
}