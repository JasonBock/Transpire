using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindDateTimeNowDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindDateTimeNowId, 
			FindDateTimeNowDescriptor.Title,
			FindDateTimeNowDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindDateTimeNowId));

	internal const string Message = "Do not use DateTime.Now.";
	internal const string Title = "Find Usage of DateTime.Now";
}