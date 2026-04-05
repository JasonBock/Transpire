using Microsoft.CodeAnalysis;
using Transpire.Analysis.Diagnostics;

namespace Transpire.Analysis.Descriptors;

internal static class CannotUseExcludedAndOrderedOnPropertyDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.CannotUseExcludedAndOrderedOnPropertyId, 
			CannotUseExcludedAndOrderedOnPropertyDescriptor.Title,
			CannotUseExcludedAndOrderedOnPropertyDescriptor.Message,
			DiagnosticConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(DescriptorIdentifiers.CannotUseExcludedAndOrderedOnPropertyId));

	internal const string Message = "Adding ordering to a property that is excluded is unnecessary.";
	internal const string Title = "Can Not Use [Excluded] and [Ordered] On a Property";
}