using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Models;

internal sealed class RecordModelGenerator
{
	internal static RecordModelGenerator Create(ITypeSymbol recordSymbol, HashSet<string> propertyNames)
	{
		var diagnostics = new List<Diagnostic>();

		// TODO: Validation and diagnostic creation goes here...
		// such as:
		// * A property name doesn't match
		// * The target isn't a record
		// * The target isn't partial, although I may not really want to check for that.

		var canGenerate = !diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error);

		return new(
			!canGenerate ? 
				null :
				new RecordModel(recordSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), [.. propertyNames]),
			[.. diagnostics]);
	}

	private RecordModelGenerator(RecordModel? model, EquatableArray<Diagnostic> diagnostics) =>
		(this.Model, this.Diagnostics) =
			(model, diagnostics);

	internal RecordModel? Model { get; }
	internal EquatableArray<Diagnostic> Diagnostics { get; }
}