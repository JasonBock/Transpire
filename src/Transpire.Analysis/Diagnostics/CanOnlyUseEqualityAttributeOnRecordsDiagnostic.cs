using Microsoft.CodeAnalysis;
using Transpire.Analysis.Descriptors;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Diagnostics;

internal static class CanOnlyUseEqualityAttributeOnRecordsDiagnostic
{
	internal static Diagnostic Create(SyntaxNode node, ITypeSymbol type, Compilation compilation) =>
		Diagnostic.Create(CanOnlyUseEqualityAttributeOnRecordsDescriptor.Create(),
			node.GetLocation(), type.GetFullyQualifiedName(compilation));
}