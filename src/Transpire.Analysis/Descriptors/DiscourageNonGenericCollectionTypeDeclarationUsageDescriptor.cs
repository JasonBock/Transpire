using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId,
			DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Title,
			DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId,
				DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Title));

	internal const string Message = "Using non-generic collections in inheritance hierarchies should be avoided.";
	internal const string Title = "Discourage Non-Generic Collection Usage in Inheritance Hierarchies";
}