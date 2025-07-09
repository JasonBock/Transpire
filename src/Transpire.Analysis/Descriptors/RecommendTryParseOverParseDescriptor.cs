using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class RecommendTryParseOverParseDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.RecommendTryParseOverParseId, 
			RecommendTryParseOverParseDescriptor.Title,
			RecommendTryParseOverParseDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.RecommendTryParseOverParseId));

	internal const string Message = "Use TryParse() instead of Parse().";
	internal const string Title = "Find Usages of Parse()";
}