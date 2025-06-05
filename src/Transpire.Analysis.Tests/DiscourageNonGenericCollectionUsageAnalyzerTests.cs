using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class DiscourageNonGenericCollectionUsageAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new DiscourageNonGenericCollectionUsageAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionUsageDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionUsageDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId,
						DiscourageNonGenericCollectionUsageDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

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

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
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

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenNonGenericCollectionIsMadeAsync(string typeName)
	{
		var objectCreationCode = $"new {typeName}()";
		var code =
			$$"""
			using System.Collections;

			internal static class Test
			{
				public static int Make()
				{
					var stuff = {{objectCreationCode}};
					return stuff.GetHashCode();
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(7, 15, 7, 15 + objectCreationCode.Length);
		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenBaseTypeIsNotNonGenericCollectionAsync()
	{
		var code =
			"""
			public sealed class Customers { }
			""";

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
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

			public class Customers
				: {{typeName}}
			{ }

			public class SpecialCustomers
				: Customers
			{ }
			""";

		var baseClassDiagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(3, 14, 3, 23);
		var subClassDiagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(7, 14, 7, 30);
		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(
			code, [baseClassDiagnostic, subClassDiagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenMethodDoesNotUseNonGenericCollectionAsync()
	{
		var code =
			"""
			internal static class Test
			{
				public static int Add(int x, int y) => x + y;
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenMethodUsesNonGenericCollectionAsync(string typeName)
	{
		var code =
			$$"""
			using System.Collections;

			internal static class Test
			{
				public static {{typeName}} Process(
					{{typeName}} items) => items;
			}
			""";

		var returnTypeDiagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(5, 16, 5, 16 + typeName.Length);
		var parameterTypeDiagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(6, 3, 6, 3 + typeName.Length);
		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(
			code, [returnTypeDiagnostic, parameterTypeDiagnostic]);
	}
}