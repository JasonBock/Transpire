using Microsoft.CodeAnalysis;
using System.Globalization;

namespace Transpire.Descriptors;

public static class MethodGenericParameterCountErrorDescriptor
{
	public static DiagnosticDescriptor Create(uint parameterCount) =>
		new(MethodGenericParameterCountErrorDescriptor.Id, MethodGenericParameterCountErrorDescriptor.Title,
			string.Format(CultureInfo.InvariantCulture, MethodGenericParameterCountErrorDescriptor.Message, parameterCount),
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				MethodGenericParameterCountErrorDescriptor.Id, MethodGenericParameterCountErrorDescriptor.Title));

	public const string Id = "TRANS16";
	public readonly static string Message = "Methods cannot have more than {0} generic parameters.";
	public const string Title = "Method Generic Parameter Count";
}