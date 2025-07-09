using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class RecommendIsNullOrWhiteSpaceAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new RecommendIsNullOrWhiteSpaceAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendIsNullOrWhiteSpaceDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendIsNullOrWhiteSpaceDescriptor.Message),
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
						DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId)),
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

		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhiteSpaceAnalyzer>(code, []);
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

		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhiteSpaceAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenMethodIsInvokedOnStringThatIsNotIsNullOrEmptyAsync()
	{
		var code =
			"""
			using System;

			public static class Test
			{
				public static void Run() 
				{ 
					_ = string.Compare("a", "b", StringComparison.OrdinalIgnoreCase);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhiteSpaceAnalyzer>(code, []);
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
			DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId, DiagnosticSeverity.Error)
			.WithSpan(5, 7, 5, 32);
		await TestAssistants.RunAnalyzerAsync<RecommendIsNullOrWhiteSpaceAnalyzer>(code, [diagnostic]);
	}
}