using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DiscourageNonGenericCollectionCreationDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DiscourageNonGenericCollectionCreationId,
			DiscourageNonGenericCollectionCreationDescriptor.Title,
			DiscourageNonGenericCollectionCreationDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DiscourageNonGenericCollectionCreationId,
				DiscourageNonGenericCollectionCreationDescriptor.Title));

	internal const string Message = "Using non-generic collections should be avoided.";
	internal const string Title = "Discourage Non-Generic Collection Usage";
}