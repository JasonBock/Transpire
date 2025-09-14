using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

internal static class DetectNonSeparatedDigitsCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new DetectNonSeparatedDigitsCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.DetectNonSeparatedDigitsId), nameof(DescriptorIdentifiers.DetectNonSeparatedDigitsId));
		});
	}

	[Test]
	public static async Task VerifyGetDetectNonSeparatedDigitsCodeFixAsync()
	{
		var originalCode =
			"""
			public static class Test
			{
				public const int Value = [|3123|];

				public static void Run() { }
			}
			""";
		var fixedCode =
			"""
			public static class Test
			{
				public const int Value = [|3_123|];

				public static void Run() { }
			}
			""";

		await TestAssistants.RunCodeFixAsync<DetectNonSeparatedDigitsAnalyzer, DetectNonSeparatedDigitsCodeFix>(
			originalCode, fixedCode, 0);
	}

	[Test]
	public static async Task VerifyGetDetectNonSeparatedDigitsCodeFixWithinArgumentAsync()
	{
		var originalCode =
			"""
			public static class Test
			{
				public static void Caller()
				{
					Callee([|10000|]);
				}
			
				public static void Callee(int value) { }
			}
			""";
		var fixedCode =
			"""
			public static class Test
			{
				public static void Caller()
				{
					Callee([|10_000|]);
				}
			
				public static void Callee(int value) { }
			}
			""";

		await TestAssistants.RunCodeFixAsync<DetectNonSeparatedDigitsAnalyzer, DetectNonSeparatedDigitsCodeFix>(
			originalCode, fixedCode, 0);
	}
}