using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class OnePropertyOrderedDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.OnePropertyOrderedId, 
			OnePropertyOrderedDescriptor.Title,
			OnePropertyOrderedDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.OnePropertyOrderedId));

	internal const string Message = "Only one property exists for equality calculations, and it is marked with [Ordered].";
	internal const string Title = "One Property Marked With [Ordered]";
}