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

		foreach (var value in new List<string>
		{
			"0x123", "0x14uL", "0x1uL",
			"0x14L", "0x1L"
		})
		{
			yield return (value, new(LiteralNumberInformationTests.GetLiteralSyntax(GetCode(value))));
		}
	}

	private static LiteralExpressionSyntax GetLiteralSyntax(string code) => 
		SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>().Single();
}