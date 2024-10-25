using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = CSharpCodeFixVerifier<FindNewDateTimeViaConstructorAnalyzer, FindNewDateTimeViaConstructorCodeFix, DefaultVerifier>;

public static class FindNewDateTimeViaConstructorCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindNewDateTimeViaConstructorCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id), nameof(FindNewDateTimeViaConstructorDescriptor.Id));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingNewDateTimeAsync()
	{
		var originalCode =
			"""
			using System;

			public static class Test
			{
				public static DateTime Make() => [|new DateTime()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			public static class Test
			{
				public static DateTime Make() => DateTime.UtcNow;
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}