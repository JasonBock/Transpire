using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Collections;
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
	public static async Task AnalyzeWhenANonGenericCollectionIsNotUsedAsync()
	{
		var code =
			"""
			internal static class Test
			{
				public static int GetCount(int value) => value;
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenGenericCollectionIsUsedAsync()
	{
		var code =
			"""
			using System.Collections.Generic;

			internal static class Test
			{
				public static int GetCount(List<string> value) => value.Count;
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, []);
	}

	[TestCase("ArrayList")]
	[TestCase("Hashtable")]
	[TestCase("Queue")]
	[TestCase("SortedList")]
	[TestCase("Stack")]
	public static async Task AnalyzeWhenNonGenericCollectionIsUsedAsync(string typeName)
	{
		var code =
			$$"""
			using System.Collections;

			internal static class Test
			{
				public static int GetCount({{typeName}} value) => value.Count;
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionUsageId, DiagnosticSeverity.Error)
			.WithSpan(5, 29, 5, 29 + typeName.Length);
		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(code, [diagnostic]);
	}
}