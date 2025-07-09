using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.VerifyDispatchProxyTProxyHasPublicParameterlessConstructorId, 
			VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Title,
			VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.VerifyDispatchProxyTProxyHasPublicParameterlessConstructorId));

	internal const string Message = "TProxy must have a internal parameterless constructor.";
	internal const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
}