using Microsoft.CodeAnalysis;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Models;

internal sealed record RecordModel
{
	internal RecordModel(ITypeSymbol recordSymbol, Compilation compilation)
	{
		this.Name = recordSymbol.Name;
		this.Namespace = recordSymbol.GetNamespace();
		this.FullyQualifiedName = recordSymbol.GetFullyQualifiedName(compilation);
		this.ClassName = RecordModel.GetClassName(recordSymbol);

		this.DeclaredAccessibility = recordSymbol.DeclaredAccessibility;

		// TODO: I want all the properties that are included -
		// that is, properties that don't have [Equality] or they do
		// and have RecordUsage.Include.
		// For each of those properties,
		// first, get the list of those that have [Equality] and a given value
		// for Order, and sort (ascending).
		// Append to this result the rest of the included properties (order doesn't matter).
		// This result needs to be included into a "Properties" property that contains
		// the name of the property, and its' FQN.
	}

	private static string GetClassName(ITypeSymbol recordSymbol)
	{
		if (recordSymbol is INamedTypeSymbol namedRecordSymbol)
		{
			if (namedRecordSymbol.TypeArguments.IsDefaultOrEmpty)
			{
				return namedRecordSymbol.Name;
			}

			var typeArgs = string.Join(", ", namedRecordSymbol.TypeArguments.Select(_ => GetClassName(_)));
			return $"{namedRecordSymbol.Name}<{typeArgs}>";
		}

		return recordSymbol.Name;
	}

	internal string ClassName { get; }
	internal string FullyQualifiedName { get; }
	internal bool IsAbstract { get; }
	internal bool IsSealed { get; }
   internal Accessibility DeclaredAccessibility { get; }
	internal string Name { get; }
	internal string Namespace { get; }
	internal TypeKind TypeKind { get; }
}