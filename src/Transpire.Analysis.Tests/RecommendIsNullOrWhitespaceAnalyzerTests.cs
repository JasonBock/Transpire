using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class RecommendIsNullOrWhitespaceAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new RecommendIsNullOrWhitespaceAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.RecommendIsNullOrWhitespaceId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendIsNullOrWhitespaceDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendIsNullOrWhitespaceDescriptor.Message),
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
						DescriptorIdentifiers.RecommendIsNullOrWhitespaceId,
						RecommendIsNullOrWhitespaceDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenNoInvocationExistsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() { }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhitespaceAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenNoIsNullOrEmptyInvocationExistsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					Test.DoWork();
				}

				private static void DoWork() { }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhitespaceAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenIsNullOrEmptyInvocationExistsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					_ = string.IsNullOrEmpty("a");
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.RecommendIsNullOrWhitespaceId, DiagnosticSeverity.Error)
			.WithSpan(5, 7, 5, 32);
		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhitespaceAnalyzer>(code, [diagnostic]);
	}
}