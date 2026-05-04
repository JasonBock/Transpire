using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindUnassignedImmutableCollectionsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindUnassignedImmutableCollectionsId,
			FindUnassignedImmutableCollectionsDescriptor.Title,
			FindUnassignedImmutableCollectionsDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindUnassignedImmutableCollectionsId));

	internal const string Message = "A return value from an immutable collection that is of the same type as the collection must be assigned.";
	internal const string Title = "No Return Value Assignment With Immutable Collections";
}