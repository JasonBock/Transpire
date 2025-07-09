using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class RecommendIsNullOrWhiteSpaceDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId,
			RecommendIsNullOrWhiteSpaceDescriptor.Title,
			RecommendIsNullOrWhiteSpaceDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId));

	internal const string Message = "Use IsNullOrWhiteSpace() instead of IsNullOrEmpty().";
	internal const string Title = "Recommend string.IsNullOrWhiteSpace()";
}