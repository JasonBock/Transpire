using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodGenericParameterCountInfoDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodGenericParameterCountInfoId, 
			MethodGenericParameterCountInfoDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountInfoDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodGenericParameterCountInfoId, 
				MethodGenericParameterCountInfoDescriptor.Title));

	internal readonly static string Message = "Consider defining this method with {0} or less generic parameters.";
	internal const string Title = "Method Generic Parameter Count";
}