using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class CanOnlyUseEqualityAttributeOnRecordsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.CanOnlyUseEqualityAttributeOnRecordsId, 
			CanOnlyUseEqualityAttributeOnRecordsDescriptor.Title,
			CanOnlyUseEqualityAttributeOnRecordsDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.CanOnlyUseEqualityAttributeOnRecordsId));

	internal const string Message = "[Equality] can only be used on a record.";
	internal const string Title = "Can Only Use [Equality] on Records";
}