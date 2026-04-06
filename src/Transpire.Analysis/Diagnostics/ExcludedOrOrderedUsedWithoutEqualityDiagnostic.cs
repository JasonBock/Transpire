using Microsoft.CodeAnalysis;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Diagnostics;

internal static class ExcludedOrOrderedUsedWithoutEqualityDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node, ITypeSymbol type, Compilation compilation) =>
		Diagnostic.Create(ExcludedOrOrderedUsedWithoutEqualityDescriptor.Create(),
			node.GetLocation(), type.GetFullyQualifiedName(compilation));
}