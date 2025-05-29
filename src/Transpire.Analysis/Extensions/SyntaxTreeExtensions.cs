using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Transpire.Analysis.Extensions;

internal static class SyntaxTreeExtensions
{
	internal static bool HasRegionDirectives(this SyntaxTree self, CancellationToken cancellationToken) =>
		self.GetRoot(cancellationToken).DescendantNodesAndTokens()
			.Any(_ => _.HasLeadingTrivia &&
				_.GetLeadingTrivia().Any(_ =>
					_.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
					_.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia));
}