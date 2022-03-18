using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodParameterCountWarningDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodParameterCountWarningDescriptor.Id, MethodParameterCountWarningDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountWarningDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountWarningDescriptor.Id, MethodParameterCountWarningDescriptor.Title));

	public const string Id = "TRANS12";
	public readonly static string Message = "Method parameter count should be equal to or less than {0}.";
	public const string Title = "Method Parameter Count";
}