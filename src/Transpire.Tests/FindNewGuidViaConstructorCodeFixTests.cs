using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Test = CSharpCodeFixTest<FindNewGuidViaConstructorAnalyzer, FindNewGuidViaConstructorCodeFix, DefaultVerifier>;

internal static class FindNewGuidViaConstructorCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindNewGuidViaConstructorCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), nameof(FindNewGuidViaConstructorDescriptor.Id));
		});
	}

	[Test]
	public static async Task VerifyGuidNewGuidCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => Guid.NewGuid();
			}
			""";
		await FindNewGuidViaConstructorCodeFixTests.VerifyAsync(originalCode, fixedCode, 0);
	}

	[Test]
	public static async Task VerifyGuidEmptyCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => Guid.Empty;
			}
			""";
		await FindNewGuidViaConstructorCodeFixTests.VerifyAsync(originalCode, fixedCode, 1);
	}

	[Test]
	public static async Task VerifyDefaultGuidCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			public static class Test
			{
				public static Guid Make() => default(Guid);
			}
			""";
		await FindNewGuidViaConstructorCodeFixTests.VerifyAsync(originalCode, fixedCode, 2);
	}

	private static async Task VerifyAsync(string originalCode, string fixedCode, int codeActionIndex)
	{
		var test = new Test
		{
			TestCode = originalCode,
			FixedCode = fixedCode,
			CodeActionIndex = codeActionIndex
		};

		await test.RunAsync();
	}
}