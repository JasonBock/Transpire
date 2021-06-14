using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class FindNewGuidViaConstructorDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(FindNewGuidViaConstructorDescriptor.Id, FindNewGuidViaConstructorDescriptor.Title,
				FindNewGuidViaConstructorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					FindNewGuidViaConstructorDescriptor.Id, FindNewGuidViaConstructorDescriptor.Title));

		public const string Id = "TRANS1";
		public const string Message = "Do not create a new Guid via its no-argument constructor.";
		public const string Title = "Creating New Guid Via No-Argument Constructor";
	}
}