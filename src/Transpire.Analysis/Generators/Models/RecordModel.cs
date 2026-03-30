using Microsoft.CodeAnalysis;
using Transpire.Analysis.Extensions;
using Transpire.Analysis.Generators.Builders;
using Transpire.Analysis.Models;

namespace Transpire.Analysis.Generators.Models;

internal sealed record RecordModel
{
	internal RecordModel(ITypeSymbol recordSymbol, Compilation compilation)
	{
		this.TypeKind = recordSymbol.TypeKind;
		this.Name = recordSymbol.Name;
		this.Namespace = recordSymbol.GetNamespace();
		this.FullyQualifiedName = recordSymbol.GetFullyQualifiedName(compilation);
		this.ClassName = RecordModel.GetClassName(recordSymbol);

		this.IsSealed = recordSymbol.IsSealed;
		this.IsAbstract = recordSymbol.IsAbstract;
		this.DeclaredAccessibility = recordSymbol.DeclaredAccessibility;

		var properties = new List<(PropertyModel, uint)>();

		foreach (var property in recordSymbol.GetMembers().OfType<IPropertySymbol>()
			.Where(property => property.Name != "EqualityContract"))
		{
			var excludedAttribute = property.GetAttributes().SingleOrDefault(
				attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, compilation.GetTypeByMetadataName("Transpire.ExcludedAttribute")));

			if (excludedAttribute is null)
			{
				var order = uint.MaxValue;

				var orderedAttribute = property.GetAttributes().SingleOrDefault(
					attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, compilation.GetTypeByMetadataName("Transpire.OrderedAttribute")));

				if (orderedAttribute is not null)
				{
					order = (uint)orderedAttribute.ConstructorArguments[0].Value!;
				}

				properties.Add((new(property.Name, property.Type.GetFullyQualifiedName(compilation)), order));
			}
		}

		this.Properties = [.. properties.OrderBy(_ => _.Item2).Select(_ => _.Item1)];
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
	internal EquatableArray<PropertyModel> Properties { get; }
	internal TypeKind TypeKind { get; }
}