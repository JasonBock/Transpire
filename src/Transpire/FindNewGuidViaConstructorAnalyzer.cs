using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Descriptors;

namespace Transpire;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindNewGuidViaConstructorAnalyzer
	: DiagnosticAnalyzer
{
	public const string CreateVersion7MemberName = "CreateVersion7";
	public const string DoesCreateVersion7Exist = nameof(DoesCreateVersion7Exist);

	private static readonly DiagnosticDescriptor rule =
		FindNewGuidViaConstructorDescriptor.Create();

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
			var guidSymbol = compilationContext.Compilation.GetTypeByMetadataName(typeof(Guid).FullName);

			if (guidSymbol is not null)
			{
				var guidConstructorSymbol = guidSymbol.InstanceConstructors.SingleOrDefault(_ => _.Parameters.Length == 0);

				if (guidConstructorSymbol is not null)
				{
					var doesCreateVersion7Exist = guidSymbol.GetMembers(FindNewGuidViaConstructorAnalyzer.CreateVersion7MemberName)
						.Any(_ => _.Name == FindNewGuidViaConstructorAnalyzer.CreateVersion7MemberName && 
							_ is IMethodSymbol);

					compilationContext.RegisterOperationAction(operationContext =>
					{
						FindNewGuidViaConstructorAnalyzer.AnalyzeOperationAction(
							operationContext, guidConstructorSymbol, doesCreateVersion7Exist);
					}, OperationKind.ObjectCreation);
				}
			}
		});
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context, IMethodSymbol guidConstructorSymbol,
		bool doesCreateVersion7Exist)
	{
		var contextInvocation = ((IObjectCreationOperation)context.Operation).Constructor;

		if (SymbolEqualityComparer.Default.Equals(contextInvocation, guidConstructorSymbol))
		{
			var properties = new Dictionary<string, string?>()
			{
				{ FindNewGuidViaConstructorAnalyzer.DoesCreateVersion7Exist, doesCreateVersion7Exist.ToString() }
			};
			context.ReportDiagnostic(Diagnostic.Create(
				FindNewGuidViaConstructorAnalyzer.rule, context.Operation.Syntax.GetLocation(),
				properties: properties.ToImmutableDictionary()));
		}
	}

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindNewGuidViaConstructorAnalyzer.rule];
}