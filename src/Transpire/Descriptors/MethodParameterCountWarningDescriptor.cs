using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class MethodParameterCountInfoDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(MethodParameterCountInfoDescriptor.Id, MethodParameterCountInfoDescriptor.Title,
			MethodParameterCountInfoDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountInfoDescriptor.Id, MethodParameterCountInfoDescriptor.Title));

	public const string Id = "TRANS11";
	public const string Message = "Consider defining this method with 4 or less parameters.";
	public const string Title = "Method Parameter Count";
}