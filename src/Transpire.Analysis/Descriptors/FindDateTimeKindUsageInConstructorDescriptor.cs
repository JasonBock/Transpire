using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindDateTimeKindUsageInConstructorDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindDateTimeKindUsageInConstructorId, 
			FindDateTimeKindUsageInConstructorDescriptor.Title,
			FindDateTimeKindUsageInConstructorDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindDateTimeKindUsageInConstructorId));

	internal const string Message = "Do not use DateTimeKind.Local or DateTimeKind.Unspecified in a DateTime constructor.";
	internal const string Title = "Using DateTimeKind in DateTime Constructor";
}