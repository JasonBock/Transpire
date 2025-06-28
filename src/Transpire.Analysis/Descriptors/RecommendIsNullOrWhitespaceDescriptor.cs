using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class RecommendIsNullOrWhitespaceDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.RecommendIsNullOrWhitespaceId,
			RecommendIsNullOrWhitespaceDescriptor.Title,
			RecommendIsNullOrWhitespaceDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.RecommendIsNullOrWhitespaceId,
				RecommendIsNullOrWhitespaceDescriptor.Title));

	internal const string Message = "Use IsNullOrWhitespace() instead of IsNullOrEmpty().";
	internal const string Title = "Recommend string.IsNullOrWhitespace()";
}