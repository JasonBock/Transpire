using Microsoft.CodeAnalysis;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Generators.Models;

internal sealed record NestedTypeModel
{
	internal NestedTypeModel(ITypeSymbol containingType)
	{
		this.TypeKind = containingType.TypeKind;
		this.ClassName = containingType.GetClassName();
		this.IsSealed = containingType.IsSealed;
		this.IsAbstract = containingType.IsAbstract;
		this.DeclaredAccessibility = containingType.DeclaredAccessibility;
	}

	internal string ClassName { get; }
	internal bool IsAbstract { get; }
	internal bool IsSealed { get; }
	internal Accessibility DeclaredAccessibility { get; }
	internal TypeKind TypeKind { get; }
}