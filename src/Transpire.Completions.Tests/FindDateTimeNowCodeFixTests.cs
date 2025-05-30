﻿using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<FindDateTimeNowAnalyzer, FindDateTimeNowCodeFix, DefaultVerifier>;

internal static class FindDateTimeNowCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindDateTimeNowCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.FindDateTimeNowId), nameof(DescriptorIdentifiers.FindDateTimeNowId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
	{
		var originalCode =
			"""
			using System;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = [|DateTime.Now|];
				}
			}
			""";
		var fixedCode =
			"""
			using System;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = DateTime.UtcNow;
				}
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}