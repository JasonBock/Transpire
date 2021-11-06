using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Id, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Title,
				VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Id, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Title));

		public const string Id = "TRANS9";
		public const string Message = "TProxy must have a public parameterless constructor.";
		public const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
	}
}