using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class VerifyDispatchProxyTProxyIsNotAbstractDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotAbstractId, 
			VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Title,
			VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotAbstractId));

	internal const string Message = "TProxy must not be abstract.";
	internal const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
}