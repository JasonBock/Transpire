using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class CallingNewGuidDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new DiagnosticDescriptor(
				CallingNewGuidDescriptor.Id, CallingNewGuidDescriptor.Title,
				CallingNewGuidDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					CallingNewGuidDescriptor.Id, CallingNewGuidDescriptor.Title));

		public const string Id = "TRANS1";
		public const string Message = "Do not create a new Guid via its' no-argument constructor.";
		public const string Title = "Creating New Guid Via No-Argument Constructor";
	}
}