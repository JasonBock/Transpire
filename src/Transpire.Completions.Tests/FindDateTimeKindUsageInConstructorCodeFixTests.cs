using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<FindDateTimeKindUsageInConstructorAnalyzer, FindDateTimeKindUsageInConstructorCodeFix, DefaultVerifier>;

internal static class FindDateTimeKindUsageInConstructorCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindDateTimeKindUsageInConstructorCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.FindDateTimeKindUsageInConstructorId), nameof(DescriptorIdentifiers.FindDateTimeKindUsageInConstructorId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenNotUsingDateTimeKindUtcAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100, [|DateTimeKind.Local|]);
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100, DateTimeKind.Utc);
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}