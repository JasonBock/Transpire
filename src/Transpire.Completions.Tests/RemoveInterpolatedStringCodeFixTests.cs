using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

using Verify = CSharpCodeFixVerifier<RemoveInterpolatedStringAnalyzer, RemoveInterpolatedStringCodeFix, DefaultVerifier>;

internal static class RemoveInterpolatedStringCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new RemoveInterpolatedStringCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.RemoveInterpolatedStringId), nameof(DescriptorIdentifiers.RemoveInterpolatedStringId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenInterpolatedStringHasNoInterpolationAsync()
	{
		var originalCode =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = [|$"This has no interpolations."|];
				}
			}
			""";
		var fixedCode =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = "This has no interpolations.";
				}
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}

	[Test]
	public static async Task VerifyGetFixesWhenLiteralInterpolatedStringHasNoInterpolationAsync()
	{
		var originalCode =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = 
			[|$@"this is
			a verbatim string."|];
				}
			}
			""";
		var fixedCode =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = 
			@"this is
			a verbatim string.";
				}
			}
			""";
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
	}
}