using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodParameterCountWarningDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodParameterCountWarningId, 
			MethodParameterCountWarningDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountWarningDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodParameterCountWarningId));

	internal readonly static string Message = "Methods should not have more than {0} parameters.";
	internal const string Title = "Method Parameter Count";
}