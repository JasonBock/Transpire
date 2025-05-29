using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Analysis.Descriptors;

internal static class MethodParameterCountErrorDescriptor
{
	internal static DiagnosticDescriptor Create(uint parameterCount) =>
		new(DescriptorIdentifiers.MethodParameterCountErrorId, 
			MethodParameterCountErrorDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountErrorDescriptor.Message, parameterCount), 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.MethodParameterCountErrorId, 
				MethodParameterCountErrorDescriptor.Title));

	internal readonly static string Message = "Methods cannot have more than {0} parameters.";
	internal const string Title = "Method Parameter Count";
}