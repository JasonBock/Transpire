using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class DeregionizeDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.DeregionizeId, 
			DeregionizeDescriptor.Title,
			DeregionizeDescriptor.Message, 
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.DeregionizeId));

	internal const string Message = "#region and #endregion directives should be avoided.";
	internal const string Title = "Using #region And/Or #endregion Directives";
}