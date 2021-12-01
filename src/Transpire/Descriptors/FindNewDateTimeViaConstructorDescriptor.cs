using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class FindNewDateTimeViaConstructorDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(FindNewDateTimeViaConstructorDescriptor.Id, FindNewDateTimeViaConstructorDescriptor.Title,
			FindNewDateTimeViaConstructorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				FindNewDateTimeViaConstructorDescriptor.Id, FindNewDateTimeViaConstructorDescriptor.Title));

	public const string Id = "TRANS3";
	public const string Message = "Do not create a new DateTime via its no-argument constructor.";
	public const string Title = "Creating New DateTime Via No-Argument Constructor";
}