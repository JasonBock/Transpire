using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodParameterCountInfoDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodParameterCountInfoDescriptor.Id, MethodParameterCountInfoDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountInfoDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountInfoDescriptor.Id, MethodParameterCountInfoDescriptor.Title));

	public const string Id = "TRANS11";
	public readonly static string Message = "Consider defining this method with {0} or less parameters.";
	public const string Title = "Method Parameter Count";
}