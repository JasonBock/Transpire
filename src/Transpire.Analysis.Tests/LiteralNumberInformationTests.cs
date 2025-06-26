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
		var information = new LiteralNumberInformation(LiteralNumberInformationTests.GetLiteralSyntax(GetCode(value)));

		Assert.Multiple(() =>
		{
			Assert.That(information.ToString(), Is.EqualTo(value));
			Assert.That(information.NeedsSeparators, Is.EqualTo(needsSeparator));
		});
	}

   [TestCase("12", 3u, "12")]
   [TestCase("1234", 3u, "1_234")]
	[TestCase("123L", 3u, "123L")]
	[TestCase("1234L", 3u, "1_234L")]
	[TestCase("4", 3u, "4")]
   [TestCase("4123", 3u, "4_123")]
   [TestCase("0x12", 3u, "0x12")]
   [TestCase("0x123", 2u, "0x1_23")]
   [TestCase("0x14uL", 4u, "0x14uL")]
   [TestCase("0x143uL", 2u, "0x1_43uL")]
   [TestCase("0x1uL", 4u, "0x1uL")]
   [TestCase("0x14L", 4u, "0x14L")]
   [TestCase("0x1L", 4u, "0x1L")]
   [TestCase("0x3d", 4u, "0x3d")]
   [TestCase("0b10", 4u, "0b10")]
   [TestCase("0b1001", 4u, "0b1001")]
   [TestCase("0b10L", 4u, "0b10L")]
   [TestCase("0b1001L", 2u, "0b10_01L")]
   [TestCase("0b10uL", 4u, "0b10uL")]
   [TestCase("0b1001uL", 4u, "0b1001uL")]
   [TestCase("3d", 4u, "3d")]
   [TestCase("3123d", 3u, "3_123d")]
   [TestCase("1e4", 3u, "1e4")]
   [TestCase("1234e4", 3u, "1_234e4")]
   [TestCase("14e10", 3u, "14e10")]
   [TestCase("1443e10", 3u, "1_443e10")]
   [TestCase("3.4", 3u, "3.4")]
   [TestCase("344.134", 3u, "344.134")]
   [TestCase("3444.134", 3u, "3_444.134")]
   [TestCase("344.1343", 3u, "344.134_3")]
   [TestCase("3.4e5", 3u, "3.4e5")]
   [TestCase("344.134e3", 3u, "344.134e3")]
   [TestCase("3444.134e3", 3u, "3_444.134e3")]
   [TestCase("344.1344e3", 3u, "344.134_4e3")]
   public static void AssertSeparatedLiteral(string value, uint spacingSize, string expected)
	{
		var information = new LiteralNumberInformation(GetLiteralSyntax(GetCode(value)));

		var separated = information.CreateSeparated(spacingSize);
		var actual = separated.ToString();
		
		Assert.That(actual, Is.EqualTo(expected));
	}

	private static string GetCode(string literalText) => $"var value = {literalText};";

   private static LiteralExpressionSyntax GetLiteralSyntax(string code) =>
		SyntaxFactory.ParseSyntaxTree(code).GetRoot().DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>().Single();
}