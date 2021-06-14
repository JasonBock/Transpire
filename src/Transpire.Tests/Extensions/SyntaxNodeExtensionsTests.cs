using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System.Linq;
using Transpire.Extensions;

namespace Transpire.Tests.Extensions
{
	public static class SyntaxNodeExtensionsTests
	{
		[TestCase("using System.Composition; public static class Test { }", "System", true)]
		[TestCase("using System; public static class Test { }", "System", true)]
		[TestCase("using NUnit.Framework; public static class Test { }", "System", false)]
		[TestCase("public static class Test { }", "System", false)]
		public static void CallHasUsing(string code, string qualifiedName, bool expectedResult)
		{
			var testNode = SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true)
				.OfType<ClassDeclarationSyntax>().Single();
			Assert.That(testNode!.HasUsing(qualifiedName), Is.EqualTo(expectedResult));
		}
	}
}