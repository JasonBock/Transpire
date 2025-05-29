using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class RemoveInterpolatedStringDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.RemoveInterpolatedStringId, 
			RemoveInterpolatedStringDescriptor.Title,
			RemoveInterpolatedStringDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.RemoveInterpolatedStringId, 
				RemoveInterpolatedStringDescriptor.Title));

	internal const string Message = "This string is not an interpolated string.";
	internal const string Title = "Find Instance of Unnecessary Interpolated Strings";
}