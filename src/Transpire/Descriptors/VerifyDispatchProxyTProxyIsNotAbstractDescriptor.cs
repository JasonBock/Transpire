using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class VerifyDispatchProxyTProxyIsNotAbstractDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Id, VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Title,
				VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
				helpLinkUri: HelpUrlBuilder.Build(
					VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Id, VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Title));

		public const string Id = "TRANS10";
		public const string Message = "TProxy must not be abstract.";
		public const string Title = "Correct Usage of TProxy Parameter for DispatchProxy.Create";
	}
}	