using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using System.Reflection.Metadata;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class DetectNonSeparatedDigitsAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new DetectNonSeparatedDigitsAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.DetectNonSeparatedDigitsId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DetectNonSeparatedDigitsDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DetectNonSeparatedDigitsDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Info),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.DetectNonSeparatedDigitsId,
						DetectNonSeparatedDigitsDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenNoLiteralExpressionExistsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() { }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DetectNonSeparatedDigitsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenLiteralExpressionExistsWithSeparatorAsync()
	{
		var code =
			"""
			public static class Test
			{
				public const int Value = 3_123;

				public static void Run() { }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DetectNonSeparatedDigitsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenLiteralExpressionExistsWithNoSeparatorsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public const int Value = 3123;

				public static void Run() { }
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DetectNonSeparatedDigitsId, DiagnosticSeverity.Info)
			.WithSpan(3, 27, 3, 31);
		await TestAssistants.RunAnalyzerAsync<DetectNonSeparatedDigitsAnalyzer>(code, [diagnostic]);
	}
}