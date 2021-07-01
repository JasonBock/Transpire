using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class FindDateTimeNowDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(FindDateTimeNowDescriptor.Id, FindDateTimeNowDescriptor.Title,
				FindDateTimeNowDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					FindDateTimeNowDescriptor.Id, FindDateTimeNowDescriptor.Title));

		public const string Id = "TRANS2";
		public const string Message = "Do not use DateTime.Now.";
		public const string Title = "Find Usage of DateTime.Now";
	}
}