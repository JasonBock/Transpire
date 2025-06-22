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
	[TestCase("0b10", false)]
	[TestCase("0b1001", true)]
	[TestCase("0b10L", false)]
	[TestCase("0b1001L", true)]
	[TestCase("0b10uL", false)]
	[TestCase("0b1001uL", true)]
	[TestCase("0x14uL", false)]
	[TestCase("0x143uL", true)]
	[TestCase("0x1uL", false)]
	[TestCase("0x14L", false)]
	[TestCase("0x1L", false)]
	[TestCase("0x3d", false)]
	[TestCase("3d", false)]
	[TestCase("3123d", true)]
	[TestCase("4", false)]
	[TestCase("4123", true)]
	[TestCase("1e4", false)]
	[TestCase("1234e4", true)]
	[TestCase("14e10", false)]
	[TestCase("1443e10", true)]
	[TestCase("3.4", false)]
	[TestCase("344.134", false)]
	[TestCase("3444.134", true)]
	[TestCase("344.1343", true)]
	[TestCase("3.4e5", false)]
	[TestCase("344.134e3", false)]
	[TestCase("3444.134e3", true)]
	[TestCase("344.1344e3", true)]
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