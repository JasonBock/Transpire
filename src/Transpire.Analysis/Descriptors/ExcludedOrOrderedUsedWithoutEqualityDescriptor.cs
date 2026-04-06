using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class ExcludedOrOrderedUsedWithoutEqualityDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.ExcludedOrOrderedUsedWithoutEqualityId, 
			ExcludedOrOrderedUsedWithoutEqualityDescriptor.Title,
			ExcludedOrOrderedUsedWithoutEqualityDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.AllPropertiesExcludedId));

	internal const string Message = "The [Excluded] or [Ordered] attribute was used on a property where the containing type did not have [Equality].";
	internal const string Title = "[Excluded] or [Ordered] Without [Equality]";
}