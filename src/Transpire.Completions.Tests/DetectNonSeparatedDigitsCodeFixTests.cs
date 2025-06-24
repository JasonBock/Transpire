using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<DetectNonSeparatedDigitsAnalyzer, DetectNonSeparatedDigitsCodeFix, DefaultVerifier>;

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

		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}