using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Transpire.Extensions;

internal static class SyntaxNodeExtensions
{
	internal static SyntaxNode? Deregionize(this SyntaxNode self)
	{
		var nodesWithRegionDirectives =
			from node in self.DescendantNodesAndTokens()
			where node.HasLeadingTrivia
			from leadingTrivia in node.GetLeadingTrivia()
			where (leadingTrivia.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
				leadingTrivia.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia)
			select node;

		var triviaToRemove = new List<SyntaxTrivia>();

		foreach (var nodeWithRegionDirective in nodesWithRegionDirectives)
		{
			var triviaList = nodeWithRegionDirective.GetLeadingTrivia();

			for (var i = 0; i < triviaList.Count; i++)
			{
				var currentTrivia = triviaList[i];

				if (currentTrivia.RawKind == (int)SyntaxKind.RegionDirectiveTrivia ||
					currentTrivia.RawKind == (int)SyntaxKind.EndRegionDirectiveTrivia)
				{
					triviaToRemove.Add(currentTrivia);

					if (i > 0)
					{
						var previousTrivia = triviaList[i - 1];

						if (previousTrivia.RawKind == (int)SyntaxKind.WhitespaceTrivia)
						{
							triviaToRemove.Add(previousTrivia);
						}
					}
				}
			}
		}

		return triviaToRemove.Count > 0 ?
			self.ReplaceTrivia(triviaToRemove,(_, _) => new SyntaxTrivia()) : 
			null;
	}

	internal static T? FindParent<T>(this SyntaxNode @this)
		where T : SyntaxNode
	{
		var parent = @this.Parent;

		while (parent is not T && parent is not null)
		{
			parent = parent.Parent;
		}

		return parent as T;
	}

	internal static bool HasUsing(this SyntaxNode self, string qualifiedName)
	{
		if (self is null) { throw new ArgumentNullException(nameof(self)); }

		var root = self;

		while (true)
		{
			if (root.Parent is not null)
			{
				root = root.Parent;
			}
			else
			{
				break;
			}
		}

		var usingNodes = root.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>();

		foreach (var usingNode in usingNodes)
		{
			if (usingNode.Name!.ToFullString().Contains(qualifiedName))
			{
				return true;
			}
		}

		return false;
	}
}