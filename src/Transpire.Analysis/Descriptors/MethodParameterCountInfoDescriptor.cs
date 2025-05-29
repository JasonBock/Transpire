using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodParameterCountInfoDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodParameterCountInfoId, 
			MethodParameterCountInfoDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountInfoDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Info, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodParameterCountInfoId, 
				MethodParameterCountInfoDescriptor.Title));

	internal readonly static string Message = "Consider defining this method with {0} or less parameters.";
	internal const string Title = "Method Parameter Count";
}