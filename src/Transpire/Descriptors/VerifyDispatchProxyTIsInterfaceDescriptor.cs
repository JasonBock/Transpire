using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class VerifyDispatchProxyTIsInterfaceDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(VerifyDispatchProxyTIsInterfaceDescriptor.Id, VerifyDispatchProxyTIsInterfaceDescriptor.Title,
			VerifyDispatchProxyTIsInterfaceDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				VerifyDispatchProxyTIsInterfaceDescriptor.Id, VerifyDispatchProxyTIsInterfaceDescriptor.Title));

	public const string Id = "TRANS7";
	public const string Message = "T must be an interface.";
	public const string Title = "Correct Usage of T Parameter for DispatchProxy.Create";
}