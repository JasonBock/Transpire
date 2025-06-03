using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

using Verify = CSharpAnalyzerVerifier<DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer, DefaultVerifier>;

internal static class DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenBaseTypeIsNotNonGenericCollectionAsync()
	{
		var code =
			"""
			public sealed class Customers { }
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenBaseTypeIsNonGenericCollectionAsync(string typeName)
	{
		var code =
			$$"""
			using System.Collections;

			public class [|Customers|]
				: {{typeName}}
			{ }

			public class [|SpecialCustomers|]
				: Customers
			{ }
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenBaseTypeHasConstraintForNonGenericCollectionAsync(string typeName)
	{
		var code =
			$$"""
			using System.Collections;

			public class [|Customers|]<T>
				: where T : Customers<T>
				where T : {{typeName}}
			{ }

			public class [|SpecialCustomers|]
				: Customers
			{ }
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}
}