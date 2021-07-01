using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Linq;
using Transpire.Descriptors;

namespace Transpire
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class FindNewDateTimeViaConstructorAnalyzer
		: DiagnosticAnalyzer
	{
		private static readonly DiagnosticDescriptor rule =
			FindNewDateTimeViaConstructorDescriptor.Create();

		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(
				GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			context.RegisterOperationAction(
				FindNewDateTimeViaConstructorAnalyzer.AnalyzeOperationAction, OperationKind.ObjectCreation);
		}

		private static void AnalyzeOperationAction(OperationAnalysisContext context)
		{
			var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;
			var dateTimeNoArgumentConstructor = context.Compilation.GetTypeByMetadataName(typeof(DateTime).FullName)!
				.InstanceConstructors.Single(_ => _.Parameters.Length == 0);

			if (SymbolEqualityComparer.Default.Equals(contextInvocation, dateTimeNoArgumentConstructor))
			{
				context.ReportDiagnostic(Diagnostic.Create(
					FindNewDateTimeViaConstructorAnalyzer.rule, context.Operation.Syntax.GetLocation()));
			}
		}

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
			ImmutableArray.Create(FindNewDateTimeViaConstructorAnalyzer.rule);
	}
}