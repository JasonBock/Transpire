using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class FindDateTimeKindUsageInConstructorDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(FindDateTimeKindUsageInConstructorDescriptor.Id, FindDateTimeKindUsageInConstructorDescriptor.Title,
				FindDateTimeKindUsageInConstructorDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					FindDateTimeKindUsageInConstructorDescriptor.Id, FindDateTimeKindUsageInConstructorDescriptor.Title));

		public const string Id = "TRANS4";
		public const string Message = "Do not use DateTimeKind.Local or DateTimeKind.Unspecified in a DateTime constructor.";
		public const string Title = "Using DateTimeKind in DateTime Constructor";
	}
}