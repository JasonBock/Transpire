using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodGenericParameterCountWarningDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodGenericParameterCountWarningId, 
			MethodGenericParameterCountWarningDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountWarningDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Warning, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodGenericParameterCountWarningId));

	internal readonly static string Message = "Methods should not have more than {0} generic parameters.";
	internal const string Title = "Method Generic Parameter Count";
}