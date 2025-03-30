using Microsoft.CodeAnalysis;

namespace Transpire.Descriptors;

public static class DeregionizeDescriptor
{
	public static DiagnosticDescriptor Create() =>
		new(DeregionizeDescriptor.Id, DeregionizeDescriptor.Title,
			DeregionizeDescriptor.Message, DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DeregionizeDescriptor.Id, DeregionizeDescriptor.Title));

	public const string Id = "TRANS17";
	public const string Message = "#region and #endregion directives should be avoided.";
	public const string Title = "Using #region And/Or #endregion Directives";
}