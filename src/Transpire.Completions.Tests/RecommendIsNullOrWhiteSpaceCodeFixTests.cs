using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

internal static class RecommendIsNullOrWhiteSpaceCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new RecommendIsNullOrWhiteSpaceCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId), nameof(DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingIsNullOrEmptyAsync()
	{
		var originalCode =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					_ = [|string.IsNullOrEmpty("a")|];
				}
			}
			""";
		var fixedCode =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					_ = string.IsNullOrWhiteSpace("a");
				}
			}
			""";

		await TestAssistants.RunCodeFixAsync<RecommendIsNullOrWhiteSpaceAnalyzer, RecommendIsNullOrWhiteSpaceCodeFix>(
			originalCode, fixedCode, 0);
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingIsNullOrEmptyWithInterpolatedStringAsync()
	{
		var originalCode =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					_ = [|string.IsNullOrEmpty($"{Test.GetLeftPart()}{Test.GetRightPart()}")|];
				}

				private static string GetLeftPart() => "a";
				private static string GetRightPart() => "b";
			}
			""";
		var fixedCode =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					_ = string.IsNullOrWhiteSpace($"{Test.GetLeftPart()}{Test.GetRightPart()}");
				}

				private static string GetLeftPart() => "a";
				private static string GetRightPart() => "b";
			}
			""";

		await TestAssistants.RunCodeFixAsync<RecommendIsNullOrWhiteSpaceAnalyzer, RecommendIsNullOrWhiteSpaceCodeFix>(
			originalCode, fixedCode, 0);
	}
}