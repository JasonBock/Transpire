using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodGenericParameterCountErrorDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodGenericParameterCountErrorId, 
			MethodGenericParameterCountErrorDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountErrorDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodGenericParameterCountErrorId));

	internal readonly static string Message = "Methods cannot have more than {0} generic parameters.";
	internal const string Title = "Method Generic Parameter Count";
}