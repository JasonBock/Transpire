using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodGenericParameterCountInfoDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodGenericParameterCountInfoDescriptor.Id, MethodGenericParameterCountInfoDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountInfoDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodGenericParameterCountInfoDescriptor.Id, MethodGenericParameterCountInfoDescriptor.Title));

	public const string Id = "TRANS14";
	public readonly static string Message = "Consider defining this method with {0} or less generic parameters.";
	public const string Title = "Method Generic Parameter Count";
}