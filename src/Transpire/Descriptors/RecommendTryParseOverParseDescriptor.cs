using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class RecommendTryParseOverParseDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(RecommendTryParseOverParseDescriptor.Id, RecommendTryParseOverParseDescriptor.Title,
			RecommendTryParseOverParseDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				RecommendTryParseOverParseDescriptor.Id, RecommendTryParseOverParseDescriptor.Title));

	public const string Id = "TRANS5";
	public const string Message = "Use TryParse() instead of Parse().";
	public const string ParameterTypeName = "TypeName";
	public const string ParameterValue = "Value";
	public const string Title = "Find Usages of Parse()";
}