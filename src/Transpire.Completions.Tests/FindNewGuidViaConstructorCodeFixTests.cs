using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

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
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.FindNewGuidViaConstructorId), nameof(DescriptorIdentifiers.FindNewGuidViaConstructorId));
		});
	}

	[Test]
	public static async Task VerifyGuidNewGuidCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => Guid.NewGuid();
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNewGuidViaConstructorAnalyzer, FindNewGuidViaConstructorCodeFix>(
			originalCode, fixedCode, 0);
	}

	[Test]
	public static async Task VerifyGuidEmptyCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => Guid.Empty;
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNewGuidViaConstructorAnalyzer, FindNewGuidViaConstructorCodeFix>(
			originalCode, fixedCode, 1);
	}

	[Test]
	public static async Task VerifyDefaultGuidCodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => default(Guid);
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNewGuidViaConstructorAnalyzer, FindNewGuidViaConstructorCodeFix>(
			originalCode, fixedCode, 2);
	}

	[Test]
	public static async Task VerifyGuidCreateVersion7CodeFixAsync()
	{
		var originalCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		var fixedCode =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => Guid.CreateVersion7();
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNewGuidViaConstructorAnalyzer, FindNewGuidViaConstructorCodeFix>(
			originalCode, fixedCode, 3);
	}
}