using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

internal static class LiteralNumberInformationTests
{
   [TestCaseSource(nameof(LiteralNumberInformationTests.GetTestInformation))]
   public static void ParseLiteralText((string text, LiteralNumberInformation information) value) => 
		Assert.That(value.information.ToString(), Is.EqualTo(value.text));

   private static IEnumerable<(string, LiteralNumberInformation)> GetTestInformation()
	{
		static string GetCode(string literalText) =>
			$"var value = {literalText};";

		yield return ("0x123", new(LiteralNumberInformationTests.GetLiteralSyntax(GetCode("0x123"))));
		//yield return ("-123", new(LiteralNumberInformationTests.GetLiteralSyntax(GetCode("-123"))));
	}

	private static LiteralExpressionSyntax GetLiteralSyntax(string code) => 
		SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>().Single();
}