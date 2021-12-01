using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = CodeFixVerifier<RecommendTryParseOverParseAnalyzer, RecommendTryParseOverParseCodeFix>;

public static class RecommendTryParseOverParseCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new RecommendTryParseOverParseCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(RecommendTryParseOverParseDescriptor.Id), nameof(RecommendTryParseOverParseDescriptor.Id));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
	{
		var originalCode =
 @"using System;

public static class ParseTest
{
	public static void MyMethod()
	{
		var result = [|int.Parse(""3"")|];
	}
}";
		var fixedCode =
 @"using System;

public static class ParseTest
{
	public static void MyMethod()
	{
		int.TryParse(""3"", out var result);
	}
}";

		await Verify.VerifyCodeFixAsync(originalCode, fixedCode).ConfigureAwait(false);
	}
}