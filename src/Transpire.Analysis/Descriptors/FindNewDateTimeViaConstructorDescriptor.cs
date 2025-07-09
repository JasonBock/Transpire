using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindNewDateTimeViaConstructorDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindNewDateTimeViaConstructorId, 
			FindNewDateTimeViaConstructorDescriptor.Title,
			FindNewDateTimeViaConstructorDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindNewDateTimeViaConstructorId));

	internal const string Message = "Do not create a new DateTime via its no-argument constructor.";
	internal const string Title = "Creating New DateTime Via No-Argument Constructor";
}