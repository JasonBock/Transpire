using Microsoft.CodeAnalysis;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Diagnostics;

internal static class NoExcludedOrOrderedUsageDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node, ITypeSymbol type, Compilation compilation) =>
		Diagnostic.Create(NoExcludedOrOrderedUsageDescriptor.Create(),
			node.GetLocation(), type.GetFullyQualifiedName(compilation));
}