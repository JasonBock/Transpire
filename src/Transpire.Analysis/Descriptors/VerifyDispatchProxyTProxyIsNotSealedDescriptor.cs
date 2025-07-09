using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class VerifyDispatchProxyTProxyIsNotSealedDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotSealedId, 
			VerifyDispatchProxyTProxyIsNotSealedDescriptor.Title,
			VerifyDispatchProxyTProxyIsNotSealedDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotSealedId));

	internal const string Message = "TProxy must not be sealed.";
	internal const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
}