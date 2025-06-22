using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

internal static class LiteralNumberInformationTests
{
	[TestCase("0x12", false)]
	[TestCase("0x123", true)]
	[TestCase("12", false)]
	[TestCase("1234", true)]
	[TestCase("0x14uL", false)]
	[TestCase("0x1uL", false)]
	[TestCase("0x14L", false)]
	[TestCase("0x1L", false)]
	[TestCase("0x3d", false)]
	[TestCase("3d", false)]
	[TestCase("4", false)]
	[TestCase("1e4", false)]
	[TestCase("14e10", false)]
	[TestCase("3.4", false)]
	[TestCase("344.134", false)]
	[TestCase("3.4e5", false)]
	[TestCase("344.134e3", false)]
	public static void ParseLiteralText(string value, bool needsSeparator)
	{
		static string GetCode(string literalText) =>
			$"var value = {literalText};";

		var information = new LiteralNumberInformation(LiteralNumberInformationTests.GetLiteralSyntax(GetCode(value)));

		Assert.Multiple(() =>
		{
			Assert.That(information.ToString(), Is.EqualTo(value));
			Assert.That(information.NeedsSeparators, Is.EqualTo(needsSeparator));
		});
	}

	private static LiteralExpressionSyntax GetLiteralSyntax(string code) => 
		SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>().Single();
}