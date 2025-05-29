using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class VerifyDispatchProxyTIsInterfaceDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.VerifyDispatchProxyTIsInterfaceId, 
			VerifyDispatchProxyTIsInterfaceDescriptor.Title,
			VerifyDispatchProxyTIsInterfaceDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.VerifyDispatchProxyTIsInterfaceId, 
				VerifyDispatchProxyTIsInterfaceDescriptor.Title));

	internal const string Message = "T must be an interface.";
	internal const string Title = "Correct Usage of T Parameter for DispatchProxy.Create";
}