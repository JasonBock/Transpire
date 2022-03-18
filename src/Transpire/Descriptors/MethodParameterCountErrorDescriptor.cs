using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodParameterCountErrorDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodParameterCountErrorDescriptor.Id, MethodParameterCountErrorDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodParameterCountErrorDescriptor.Message, parameterCount), 
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodParameterCountErrorDescriptor.Id, MethodParameterCountErrorDescriptor.Title));

	public const string Id = "TRANS13";
	public readonly static string Message = "Methods cannot have more than {0} parameters.";
	public const string Title = "Method Parameter Count";
}