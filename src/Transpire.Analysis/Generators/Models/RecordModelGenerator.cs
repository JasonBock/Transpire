using Microsoft.CodeAnalysis;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Generators.Models;

internal sealed class RecordModelGenerator
{
	internal static RecordModelGenerator Create(SyntaxNode node, ITypeSymbol recordSymbol, Compilation compilation)
	{
		if (recordSymbol.HasErrors())
		{
			// This one will stop everything. There's no need to move on
			// if the given type is in error.
			return new(null, []);
		}

		var diagnostics = new List<Diagnostic>();
		var model = new RecordModel(node, recordSymbol, compilation, diagnostics);

		var canGenerate = !diagnostics.Any(_ => _.Severity == DiagnosticSeverity.Error);

		return new(
			!canGenerate ?
				null :
				model,
			[.. diagnostics]);
	}

	private RecordModelGenerator(RecordModel? model, EquatableArray<Diagnostic> diagnostics) =>
		(this.Model, this.Diagnostics) =
			(model, diagnostics);

	internal RecordModel? Model { get; }
	internal EquatableArray<Diagnostic> Diagnostics { get; }
}