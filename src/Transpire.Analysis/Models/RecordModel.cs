namespace Transpire.Analysis.Models;

internal sealed record RecordModel(
	string RecordFullyQualifiedName, EquatableArray<string> PropertyNames);