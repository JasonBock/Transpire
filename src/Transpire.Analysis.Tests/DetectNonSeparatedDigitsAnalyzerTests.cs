using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
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
						DescriptorIdentifiers.DetectNonSeparatedDigitsId)),
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

	[Test]
	public static async Task AnalyzeWhenLiteralExpressionExistsWithNoSeparatorsAndDiagnosticsExistAsync()
	{
		var code =
			"""
			public static class Test
			{
				public const int Value = 0b10011021;

				public static void Run() { }
			}
			""";

		// This diagnostic is a compilation error, not our analyzer diagnostic.
		var diagnostic = DiagnosticResult.CompilerError("CS1003")
			.WithSpan(3, 35, 3, 37).WithArguments(",");
		await TestAssistants.RunAnalyzerAsync<DetectNonSeparatedDigitsAnalyzer>(code, [diagnostic]);
	}

	[TestCase("int", "312", false)]
	[TestCase("int", "3123", true)]
	[TestCase("int", "0b11", false)]
	[TestCase("int", "0B11", false)]
	[TestCase("int", "0b111", true)]
	[TestCase("int", "0B111", true)]
	[TestCase("int", "0x11", false)]
	[TestCase("int", "0X11", false)]
	[TestCase("int", "0x111", true)]
	[TestCase("int", "0X111", true)]
	[TestCase("double", "3.12", false)]
	[TestCase("double", "312.312", false)]
	[TestCase("double", "3312.312", true)]
	[TestCase("double", "312.3123", true)]
	[TestCase("double", ".12", false)]
	[TestCase("double", ".1234", true)]
	public static async Task AnalyzeWhenLiteralExpressionExistsWithNoSeparatorsAsync(
		string literalType, string literalText, bool shouldCreateDiagnostic)
	{
		var code =
			$$"""
			public static class Test
			{
				public const {{literalType}} Value = 
					{{literalText}};

				public static void Run() { }
			}
			""";

		var diagnostics = shouldCreateDiagnostic ?
			new List<DiagnosticResult>
			{
				new DiagnosticResult(
					DescriptorIdentifiers.DetectNonSeparatedDigitsId, DiagnosticSeverity.Info)
					.WithSpan(4, 3, 4, 3 + literalText.Length)
			} : [];

		await TestAssistants.RunAnalyzerAsync<DetectNonSeparatedDigitsAnalyzer>(code, diagnostics);
	}
}