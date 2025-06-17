using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Transpire.Analysis.Extensions;

namespace Transpire.Analysis.Tests.Extensions;

internal static class LiteralExpressionSyntaxExtensionsTests
{
	[Test]
	public static void ContainsDiagnosticsWhenLiteralIsCorrect()
	{
		var code =
			"""
			public static class Test
			{
				public static void Do()
				{
					var x = 123;
				}
			}
			""";

		LiteralExpressionSyntaxExtensionsTests.ContainsDiagnostics(
			code, false);
	}

	[Test]
	public static void ContainsDiagnosticsWhenMemberDeclarationIsIncorrect()
	{
		var code =
			"""
			public static class Test
			{
				public const int x = 0b100121;
			}
			""";

		LiteralExpressionSyntaxExtensionsTests.ContainsDiagnostics(
			code, true);
	}

	[Test]
	public static void ContainsDiagnosticsWhenVariableDeclarationIsIncorrect()
	{
		var code =
			"""
			public static class Test
			{
				public static void Do()
				{
					var x = 0b100121;
				}
			}
			""";

		LiteralExpressionSyntaxExtensionsTests.ContainsDiagnostics(
			code, true);
	}

	[Test]
	public static void ContainsDiagnosticsWhenLiteralIsCorrectAndWithinIncorrectCode()
	{
		var code =
			"""
			public static class Test
			{
				public static void Do()
				{
					var x = 0b100111;
			}
			""";

		LiteralExpressionSyntaxExtensionsTests.ContainsDiagnostics(
			code, false);
	}

	private static void ContainsDiagnostics(string code, bool mayContainDiagnostics)
	{
		var unit = SyntaxFactory.ParseCompilationUnit(code);
		var literals = unit.DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>();

		foreach (var literal in literals)
		{
			Assert.That(literal.MayContainDiagnostics(), Is.EqualTo(mayContainDiagnostics));
		}
	}
}