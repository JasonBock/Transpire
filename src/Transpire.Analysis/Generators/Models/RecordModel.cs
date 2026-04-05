using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Generators.Models;

internal sealed record RecordModel
{
	internal RecordModel(SyntaxNode node, ITypeSymbol recordSymbol, Compilation compilation, List<Diagnostic> diagnostics)
	{
		// Validation and diagnostic creation, such as:
		/*
		* If `[Equality]` exists
			* On a non-record, error
			* Both `[Excluded]` and `[Ordered]` exist on a property, error
			* The type doesn't have any properties marked with either `[Excluded]` or `[Ordered]`, error
			* If every property ends up being excluded, error
			* If there's only one property, and it has `[Ordered]`, error
		*/

		this.TypeKind = recordSymbol.TypeKind;
		this.Name = recordSymbol.Name;
		this.Namespace = recordSymbol.GetNamespace();
		this.FullyQualifiedName = recordSymbol.GetFullyQualifiedName(compilation);
		this.ClassName = recordSymbol.GetClassName();
		this.IsSealed = recordSymbol.IsSealed;
		this.IsAbstract = recordSymbol.IsAbstract;
		this.DeclaredAccessibility = recordSymbol.DeclaredAccessibility;

		var nestedTypes = new List<NestedTypeModel>();

		var containingType = recordSymbol.ContainingType;

		while (containingType is not null)
		{
			nestedTypes.Insert(0, new(containingType));
			containingType = containingType.ContainingType;
		}

		this.NestedTypes = [.. nestedTypes];

		var properties = new List<(PropertyModel, uint)>();
		var usedExcludedAttribute = false;
		var usedOrderedAttribute = false;

		foreach (var property in recordSymbol.GetMembers().OfType<IPropertySymbol>()
			.Where(property => property.Name != "EqualityContract"))
		{
			var excludedAttribute = property.GetAttributes().SingleOrDefault(
				attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, compilation.GetTypeByMetadataName("Transpire.ExcludedAttribute")));
			var orderedAttribute = property.GetAttributes().SingleOrDefault(
				attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, compilation.GetTypeByMetadataName("Transpire.OrderedAttribute")));

			usedExcludedAttribute |= excludedAttribute is not null;
			usedOrderedAttribute |= orderedAttribute is not null;

			if (excludedAttribute is not null && orderedAttribute is not null)
			{
				diagnostics.Add(CannotUseExcludedAndOrderedOnPropertyDiagnostic.Create(node, recordSymbol, compilation));
			}

			if (excludedAttribute is null)
			{
				var wasOrdered = false;
				var order = uint.MaxValue;

				if (orderedAttribute is not null)
				{
					order = (uint)orderedAttribute.ConstructorArguments[0].Value!;
					wasOrdered = true;
				}

				properties.Add((new(property.Name, property.Type.GetFullyQualifiedName(compilation), wasOrdered), order));
			}
		}

		this.Properties = [.. properties.OrderBy(_ => _.Item2).Select(_ => _.Item1)];

		if (!usedExcludedAttribute && !usedOrderedAttribute)
		{
			diagnostics.Add(CannotUseExcludedAndOrderedOnPropertyDiagnostic.Create(node, recordSymbol, compilation));
		}

		if (this.Properties.Length == 0)
		{
			diagnostics.Add(AllPropertiesExcludedDiagnostic.Create(node, recordSymbol, compilation));
		}
		else if (this.Properties.Length == 1 && this.Properties[0].WasOrdered)
		{
			diagnostics.Add(AllPropertiesExcludedDiagnostic.Create(node, recordSymbol, compilation));
		}

		if (!recordSymbol.IsRecord)
		{
			diagnostics.Add(CanOnlyUseEqualityAttributeOnRecordsDiagnostic.Create(node, recordSymbol, compilation));
		}
	}

	internal string ClassName { get; }
	internal string FullyQualifiedName { get; }
	internal bool IsAbstract { get; }
	internal bool IsSealed { get; }
	internal Accessibility DeclaredAccessibility { get; }
	internal string Name { get; }
	internal string Namespace { get; }
	internal EquatableArray<NestedTypeModel> NestedTypes { get; }
	internal EquatableArray<PropertyModel> Properties { get; }
	internal TypeKind TypeKind { get; }
}