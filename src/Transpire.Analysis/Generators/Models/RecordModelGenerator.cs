using Microsoft.CodeAnalysis;
using Transpire.Analysis.Generators.Models;

namespace Transpire.Analysis.Models;

internal sealed class RecordModelGenerator
{
	internal static RecordModelGenerator Create(ITypeSymbol recordSymbol, Compilation compilation)
	{
		var diagnostics = new List<Diagnostic>();

		// TODO: Validation and diagnostic creation goes here...
		// such as:
		//	* If `[Equality]` exists on a property that's defined on a type that isn't a record, error
		//	* If `[EqualityMarkup]` exists on a non-record, error
		//	* If `[EqualityMarkup]` exists on a record that doesn't have any properties marked with `[Equality]`, error

		var canGenerate = !diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error);

		return new(
			!canGenerate ? 
				null :
				new RecordModel(recordSymbol, compilation),
			[.. diagnostics]);
	}

	private RecordModelGenerator(RecordModel? model, EquatableArray<Diagnostic> diagnostics) =>
		(this.Model, this.Diagnostics) =
			(model, diagnostics);

	internal RecordModel? Model { get; }
	internal EquatableArray<Diagnostic> Diagnostics { get; }
}