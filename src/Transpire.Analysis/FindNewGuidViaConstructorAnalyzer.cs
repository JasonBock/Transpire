using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis;

/// <summary>
/// An analyzer that finds <see cref="Guid()" /> usage.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FindNewGuidViaConstructorAnalyzer
	: DiagnosticAnalyzer
{
	/// <summary>
	/// Specifies the name of the <c>CreateVersion7</c> method name on <see cref="Guid"/>.
	/// </summary>
	public const string CreateVersion7MemberName = "CreateVersion7";

	private static readonly DiagnosticDescriptor rule =
		FindNewGuidViaConstructorDescriptor.Create();

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
				{ Constants.DoesCreateVersion7ExistKey, doesCreateVersion7Exist.ToString() }
			};
			context.ReportDiagnostic(Diagnostic.Create(
				FindNewGuidViaConstructorAnalyzer.rule, context.Operation.Syntax.GetLocation(),
				properties: properties.ToImmutableDictionary()));
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[FindNewGuidViaConstructorAnalyzer.rule];
}