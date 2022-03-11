using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class MethodParameterCountWarningDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(MethodParameterCountWarningDescriptor.Id, MethodParameterCountWarningDescriptor.Title,
			MethodParameterCountWarningDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountWarningDescriptor.Id, MethodParameterCountWarningDescriptor.Title));

	public const string Id = "TRANS12";
	public const string Message = "Method parameter count should be equal to or less than 16.";
	public const string Title = "Method Parameter Count";
}