using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DetectNonSeparatedDigitsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DetectNonSeparatedDigitsId,
			DetectNonSeparatedDigitsDescriptor.Title,
			DetectNonSeparatedDigitsDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DetectNonSeparatedDigitsId));

	internal const string Message = "Numbers should use digit separators.";
	internal const string Title = "Non-Separated Numbers Detected";
}