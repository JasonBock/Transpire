using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using Transpire.Descriptors;
using Microsoft.CodeAnalysis.Operations;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindingDateTimeNowAnalyzer
		: DiagnosticAnalyzer
	{
		private static DiagnosticDescriptor rule = FindingDateTimeNowDescriptor.Create();

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => 
			ImmutableArray.Create(FindingDateTimeNowAnalyzer.rule);

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			context.RegisterOperationAction(
				FindingDateTimeNowAnalyzer.AnalyzeOperationAction, OperationKind.PropertyReference);

			//context.RegisterSyntaxNodeAction(
			//	AnalyzeSimpleMemberAccessExpression, SyntaxKind.SimpleMemberAccessExpression);
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context)
		{
			var contextReference = ((IPropertyReferenceOperation)context.Operation).Property;
			var dateTimeNowSymbol = context.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName)!
				.GetMembers(nameof(DateTime.Now));

			if(dateTimeNowSymbol.Length == 1 &&
				SymbolEqualityComparer.Default.Equals(contextReference, dateTimeNowSymbol[0]))
			{
				context.ReportDiagnostic(Diagnostic.Create(FindingDateTimeNowAnalyzer.rule,
					context.Operation.Syntax.GetLocation()));
			}
		}

		//private static void AnalyzeSimpleMemberAccessExpression(SyntaxNodeAnalysisContext context)
		//{
		//	var memberNode = (MemberAccessExpressionSyntax)context.Node;

		//	if (memberNode.OperatorToken.IsKind(SyntaxKind.DotToken) &&
		//		memberNode.Name.Identifier.ValueText == "Now")
		//	{
		//		var symbol = context.SemanticModel.GetSymbolInfo(memberNode.Name).Symbol;
		//		var dateTimeSymbol = context.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName);

		//		if (SymbolEqualityComparer.Default.Equals(symbol, dateTimeSymbol))
		//		{
		//			context.ReportDiagnostic(Diagnostic.Create(FindingDateTimeNowAnalyzer.rule,
		//				memberNode.Name.Identifier.GetLocation()));
		//		}
		//	}
		//}
	}
}