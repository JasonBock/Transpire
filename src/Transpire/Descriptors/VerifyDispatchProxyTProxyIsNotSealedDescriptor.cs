using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class VerifyDispatchProxyTProxyIsNotSealedDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(VerifyDispatchProxyTProxyIsNotSealedDescriptor.Id, VerifyDispatchProxyTProxyIsNotSealedDescriptor.Title,
				VerifyDispatchProxyTProxyIsNotSealedDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
				helpLinkUri: HelpUrlBuilder.Build(
					VerifyDispatchProxyTProxyIsNotSealedDescriptor.Id, VerifyDispatchProxyTProxyIsNotSealedDescriptor.Title));

		public const string Id = "TRANS8";
		public const string Message = "TProxy must not be sealed.";
		public const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
	}
}