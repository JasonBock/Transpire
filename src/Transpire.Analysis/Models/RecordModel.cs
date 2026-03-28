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
		this.ClassName = this.GetClassName(recordSymbol);

		this.DeclaredAccessibility = recordSymbol.DeclaredAccessibility; 
	}

	private string GetClassName(ITypeSymbol recordSymbol)
	{
		if (recordSymbol is INamedTypeSymbol namedRecordSymbol)
		{
			if (namedRecordSymbol.TypeArguments.IsDefaultOrEmpty)
			{
				return namedRecordSymbol.Name;
			}

			var typeArgs = string.Join(", ", namedRecordSymbol.TypeArguments.Select(_ => this.GetClassName(_)));
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