using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class NoExcludedOrOrderedUsageDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.NoExcludedOrOrderedUsageId, 
			NoExcludedOrOrderedUsageDescriptor.Title,
			NoExcludedOrOrderedUsageDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.NoExcludedOrOrderedUsageId));

	internal const string Message = "If [Equality] is used, at least one property should have [Excluded] or [Ordered].";
	internal const string Title = "No [Excluded] or [Ordered] Usage";
}