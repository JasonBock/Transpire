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
			Assert.That(diagnostics, Has.Length.EqualTo(2), nameof(diagnostics.Length));

			var creationDiagnostic = diagnostics.Single(_ => _.Id == DescriptorIdentifiers.DiscourageNonGenericCollectionCreationId);

			Assert.That(creationDiagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionCreationDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(creationDiagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionCreationDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(creationDiagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(creationDiagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(creationDiagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(creationDiagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.DiscourageNonGenericCollectionCreationId,
						DiscourageNonGenericCollectionCreationDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));

			var typeDeclarationDiagnostic = diagnostics.Single(_ => _.Id == DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId);

			Assert.That(typeDeclarationDiagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(typeDeclarationDiagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(typeDeclarationDiagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(typeDeclarationDiagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(typeDeclarationDiagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(typeDeclarationDiagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId,
						DiscourageNonGenericCollectionTypeDeclarationUsageDescriptor.Title)),
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
			DescriptorIdentifiers.DiscourageNonGenericCollectionCreationId, DiagnosticSeverity.Error)
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
			DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId, DiagnosticSeverity.Error)
			.WithSpan(3, 14, 3, 23);
		var subClassDiagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DiscourageNonGenericCollectionTypeDeclarationUsageId, DiagnosticSeverity.Error)
			.WithSpan(7, 14, 7, 30);
		await TestAssistants.RunAnalyzerAsync<DiscourageNonGenericCollectionUsageAnalyzer>(
			code, [baseClassDiagnostic, subClassDiagnostic]);
	}
}