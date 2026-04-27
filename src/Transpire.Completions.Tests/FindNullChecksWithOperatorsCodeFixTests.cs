using NUnit.Framework;
using Transpire.Analysis.Analyzers;

namespace Transpire.Completions.Tests;

internal static class FindNullChecksWithOperatorsCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new FindNullChecksWithOperatorsCodeFix();
		var ids = fix.FixableDiagnosticIds;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.FindNullChecksWithOperatorsId), nameof(DescriptorIdentifiers.FindNullChecksWithOperatorsId));
		}
	}

	[Test]
	public static async Task VerifyEqualsCodeFixAsync()
	{
		var originalCode =
			"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					[|value == null|];
			}
			""";
		var fixedCode =
			"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					value is null;
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNullChecksWithOperatorsAnalyzer, FindNullChecksWithOperatorsCodeFix>(
			originalCode, fixedCode, 0);
	}

	[Test]
	public static async Task VerifyNotEqualsCodeFixAsync()
	{
		var originalCode =
			"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					[|value != null|];
			}
			""";
		var fixedCode =
			"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					value is not null;
			}
			""";

		await TestAssistants.RunCodeFixAsync<FindNullChecksWithOperatorsAnalyzer, FindNullChecksWithOperatorsCodeFix>(
			originalCode, fixedCode, 0);
	}
}