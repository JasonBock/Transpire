﻿using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<FindNewDateTimeViaConstructorAnalyzer, FindNewDateTimeViaConstructorCodeFix, DefaultVerifier>;

internal static class FindNewDateTimeViaConstructorCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindNewDateTimeViaConstructorCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.FindNewDateTimeViaConstructorId), nameof(DescriptorIdentifiers.FindNewDateTimeViaConstructorId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingNewDateTimeAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => [|new DateTime()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => DateTime.UtcNow;
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}