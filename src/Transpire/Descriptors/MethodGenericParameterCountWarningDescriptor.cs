using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodGenericParameterCountWarningDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodGenericParameterCountWarningDescriptor.Id, MethodGenericParameterCountWarningDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountWarningDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodGenericParameterCountWarningDescriptor.Id, MethodGenericParameterCountWarningDescriptor.Title));

	public const string Id = "TRANS15";
	public readonly static string Message = "Methods should not have more than {0} generic parameters.";
	public const string Title = "Method Generic Parameter Count";
}