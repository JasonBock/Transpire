using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Transpire.Extensions
{
	internal static class SyntaxNodeExtensions
	{
		internal static bool HasUsing(this SyntaxNode self, string qualifiedName)
		{
			if (self is null)
			{
				return false;
			}

			// TODO: Not sure this is correct. If you have "System.Composition",
			// I don't think you need to add "System".
			if (self.Kind() == SyntaxKind.UsingDirective)
			{
				var usingNode = (UsingDirectiveSyntax)self;

				if (usingNode.Name.ToFullString() == qualifiedName)
				{
					return true;
				}
			}

			return self.ChildNodes().Where(_ => _.HasUsing(qualifiedName)).Any();
		}
	}
}