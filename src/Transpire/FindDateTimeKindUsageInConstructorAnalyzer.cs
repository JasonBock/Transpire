using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindDateTimeKindUsageInConstructorAnalyzer
		: DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor rule =
			FindDateTimeKindUsageInConstructorDescriptor.Create();

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(compilationContext =>
			{
				var dateTimeSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName);
				var dateTimeKindSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(DateTimeKind).FullName);

				if(dateTimeSymbol is null || dateTimeKindSymbol is null)
				{
					return;
				}

				compilationContext.RegisterOperationAction(operationContext =>
				{
					FindDateTimeKindUsageInConstructorAnalyzer.AnalyzeOperationAction(
						operationContext, dateTimeSymbol, dateTimeKindSymbol);
				}, OperationKind.ObjectCreation);
			});
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context,
			INamedTypeSymbol dateTimeSymbol, INamedTypeSymbol dateTimeKindSymbol)
		{
			var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

			if (SymbolEqualityComparer.Default.Equals(contextInvocation!.ContainingType, dateTimeSymbol))
			{
				var invocationSyntax = (BaseObjectCreationExpressionSyntax)context.Operation.Syntax;

				for (var i = 0; i < contextInvocation!.Parameters.Length; i++)
				{
					var parameter = contextInvocation!.Parameters[i];

					if (SymbolEqualityComparer.Default.Equals(parameter.Type, dateTimeKindSymbol))
					{
						var argument = invocationSyntax.ArgumentList!.Arguments[i];

						if (argument.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
						{
							var argumentExpressionSymbol = context.Operation.SemanticModel
								.GetSymbolInfo(argument.Expression).Symbol as IFieldSymbol;

							if (argumentExpressionSymbol?.Name != nameof(DateTimeKind.Utc))
							{
								context.ReportDiagnostic(Diagnostic.Create(
									FindDateTimeKindUsageInConstructorAnalyzer.rule, argument.GetLocation()));
							}
						}
					}
				}
			}
		}

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(FindDateTimeKindUsageInConstructorAnalyzer.rule);
	}
}