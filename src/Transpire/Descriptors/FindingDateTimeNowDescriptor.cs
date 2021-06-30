using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class FindingDateTimeNowDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(FindingDateTimeNowDescriptor.Id, FindingDateTimeNowDescriptor.Title,
				FindingDateTimeNowDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					FindingDateTimeNowDescriptor.Id, FindingDateTimeNowDescriptor.Title));

		public const string Id = "TRANS2";
		public const string Message = "Do not use DateTime.Now.";
		public const string Title = "Find Usage of DateTime.Now";
	}
}