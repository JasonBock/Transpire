using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class AllPropertiesExcludedDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.AllPropertiesExcludedId, 
			AllPropertiesExcludedDescriptor.Title,
			AllPropertiesExcludedDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.AllPropertiesExcludedId));

	internal const string Message = "All properties were excluded from equality calculation.";
	internal const string Title = "All Properties Excluded";
}