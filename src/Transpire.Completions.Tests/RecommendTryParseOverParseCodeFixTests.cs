using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<RecommendTryParseOverParseAnalyzer, RecommendTryParseOverParseCodeFix, DefaultVerifier>;

internal static class RecommendTryParseOverParseCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new RecommendTryParseOverParseCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.RecommendTryParseOverParseId), nameof(DescriptorIdentifiers.RecommendTryParseOverParseId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class ParseTest
			{
				public static void MyMethod()
				{
					var result = [|int.Parse("3")|];
				}
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class ParseTest
			{
				public static void MyMethod()
				{
					int.TryParse("3", out var result);
				}
			}
			""";

		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}