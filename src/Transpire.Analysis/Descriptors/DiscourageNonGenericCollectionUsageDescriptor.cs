using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DiscourageNonGenericCollectionUsageDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId,
			DiscourageNonGenericCollectionUsageDescriptor.Title,
			DiscourageNonGenericCollectionUsageDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId,
				DiscourageNonGenericCollectionUsageDescriptor.Title));

	internal const string Message = "Using non-generic collections should be avoided.";
	internal const string Title = "Discourage Non-Generic Collection Usage";
}