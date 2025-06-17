using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Transpire.Analysis.Extensions;

internal static class LiteralExpressionSyntaxExtensions
{
	// This is a heuristic. A LiteralExpressionSyntax will not have diagnostics
	// on itself, but the "parent" will. This list should hopefully associate
	// any diagnostics that exist with the literal on a parent node.
	internal static bool MayContainDiagnostics(this LiteralExpressionSyntax self)
	{
		if (self.ContainsDiagnostics)
		{
			return true;
		}

		var parent = self.Parent;

		while (parent is not null)
		{
			if ((parent is LocalDeclarationStatementSyntax ||
				parent is ArgumentListSyntax ||
				parent is MemberDeclarationSyntax ||
				parent is BlockSyntax))
			{
				return parent.ContainsDiagnostics;
			}

			parent = parent.Parent;
		}

		return false;
	}
}