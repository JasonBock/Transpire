using Microsoft.CodeAnalysis;

namespace Transpire.Analysis.Descriptors;

internal static class FindNullChecksWithOperatorsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(DescriptorIdentifiers.FindNullChecksWithOperatorsId,
			FindNullChecksWithOperatorsDescriptor.Title,
			FindNullChecksWithOperatorsDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				DescriptorIdentifiers.FindNullChecksWithOperatorsId));

	internal const string Message = "Null checks should use the \"is\" pattern.";
	internal const string Title = "Null Check With Operator";
}