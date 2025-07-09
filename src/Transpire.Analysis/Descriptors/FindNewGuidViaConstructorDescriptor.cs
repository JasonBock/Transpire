using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindNewGuidViaConstructorDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindNewGuidViaConstructorId, 
			FindNewGuidViaConstructorDescriptor.Title,
			FindNewGuidViaConstructorDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindNewGuidViaConstructorId));

	internal const string Message = "Do not create a new Guid via its no-argument constructor.";
	internal const string Title = "Creating New Guid Via No-Argument Constructor";
}