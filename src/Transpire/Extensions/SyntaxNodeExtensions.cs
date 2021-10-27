using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Linq;
using System;

namespace Transpire.Extensions
{
	internal static class SyntaxNodeExtensions
	{
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

			while(true)
			{
				if(root.Parent is not null)
				{
					root = root.Parent;
				}
				else
				{
					break;
				}
			}

			var usingNodes = root.DescendantNodes(_ => true).OfType<UsingDirectiveSyntax>();

			foreach(var usingNode in usingNodes)
			{
				if(usingNode.Name.ToFullString().Contains(qualifiedName))
				{
					return true;
				}
			}

			return false;
		}
	}
}