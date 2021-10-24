using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors
{
	public static class RemoveInterpolatedStringDescriptor
	{
		public static DiagnosticDescriptor Create() =>
			new(RemoveInterpolatedStringDescriptor.Id, RemoveInterpolatedStringDescriptor.Title,
				RemoveInterpolatedStringDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
				helpLinkUri: HelpUrlBuilder.Build(
					RemoveInterpolatedStringDescriptor.Id, RemoveInterpolatedStringDescriptor.Title));

		public const string Id = "TRANS6";
		public const string Message = "This string is not an interpolated string.";
		public const string Title = "Find Instance of Unnecessary Interpolated Strings";
	}
}