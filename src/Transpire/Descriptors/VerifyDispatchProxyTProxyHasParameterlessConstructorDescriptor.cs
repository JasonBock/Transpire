using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Id, VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Title,
				VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
				helpLinkUri: HelpUrlBuilder.Build(
					VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Id, VerifyDispatchProxyTProxyHasParameterlessConstructorDescriptor.Title));

		public const string Id = "TRANS9";
		public const string Message = "TProxy must have a parameterless constructor.";
		public const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
	}
}