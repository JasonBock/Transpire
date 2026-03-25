using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Models;

internal sealed record RecordModel(
	string? Namespace, string FullyQualifiedName, 
	Accessibility DeclaredAccessibility, EquatableArray<string> ExcludedPropertyNames);