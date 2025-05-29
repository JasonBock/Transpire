using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Transpire.Completions.Extensions;

namespace Transpire.Completions.Tests.Extensions;

internal static class SyntaxNodeExtensionsTests
{
	[TestCase("using System.Composition; internal static class Test { }", "System", true)]
	[TestCase("using System; internal static class Test { }", "System", true)]
	[TestCase("using NUnit.Framework; internal static class Test { }", "System", false)]
	[TestCase("internal static class Test { }", "System", false)]
	public static void CallHasUsing(string code, string qualifiedName, bool expectedResult)
	{
		var testNode = SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true)
			.OfType<ClassDeclarationSyntax>().Single();
		Assert.That(testNode!.HasUsing(qualifiedName), Is.EqualTo(expectedResult));
	}
}