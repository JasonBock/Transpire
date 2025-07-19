using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DetectGotoUsageDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DetectGotoUsageId,
			DetectGotoUsageDescriptor.Title,
			DetectGotoUsageDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DetectGotoUsageId));

	internal const string Message = "Strongly consider not using gotos in C#.";
	internal const string Title = "Goto Detected";
}