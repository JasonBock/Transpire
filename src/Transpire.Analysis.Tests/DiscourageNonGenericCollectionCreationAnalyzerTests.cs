using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

using Verify = CSharpAnalyzerVerifier<DiscourageNonGenericCollectionCreationAnalyzer, DefaultVerifier>;

internal static class DiscourageNonGenericCollectionCreationAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenNothingIsMadeAsync()
	{
		var code =
			"""
			internal static class Test
			{
				public static int Make() => 1 + 2;
			}
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenGenericCollectionIsMadeAsync()
	{
		var code =
			"""
			using System.Collections.Generic;

			internal static class Test
			{
				public static int Make()
				{
					var stuff = new List<string>();
					return stuff.GetHashCode();
				}
			}
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenNonGenericCollectionIsMadeAsync(string typeName)
	{
		var code =
			$$"""
			using System.Collections;

			internal static class Test
			{
				public static int Make()
				{
					var stuff = [|new {{typeName}}()|];
					return stuff.GetHashCode();
				}
			}
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}
}