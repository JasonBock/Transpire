using Microsoft.CodeAnalysis;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Diagnostics;

internal static class OnePropertyOrderedDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node, ITypeSymbol type, Compilation compilation) =>
		Diagnostic.Create(OnePropertyOrderedDescriptor.Create(),
			node.GetLocation(), type.GetFullyQualifiedName(compilation));
}