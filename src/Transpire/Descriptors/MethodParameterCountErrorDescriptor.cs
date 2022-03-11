using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class MethodParameterCountErrorDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(MethodParameterCountErrorDescriptor.Id, MethodParameterCountErrorDescriptor.Title,
			MethodParameterCountErrorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountErrorDescriptor.Id, MethodParameterCountErrorDescriptor.Title));

	public const string Id = "TRANS13";
	public const string Message = "Methods should not have more than 8192 parameters.";
	public const string Title = "Method Parameter Count";
}