using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class FindNewDateTimeViaConstructorAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindNewDateTimeViaConstructorAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.FindNewDateTimeViaConstructorId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Message),
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
						DescriptorIdentifiers.FindNewDateTimeViaConstructorId)),
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

		await TestAssistants.RunAnalyzerAsync<FindNewDateTimeViaConstructorAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenNoDateIsMadeAsync()
	{
		var code =
			"""
			internal static class Test
			{
				public static string Make() => new string('a', 1);
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindNewDateTimeViaConstructorAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeIsMadeViaParametersAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100, DateTimeKind.Local);
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindNewDateTimeViaConstructorAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeIsMadeViaNoArgumentConstructorAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.FindNewDateTimeViaConstructorId, DiagnosticSeverity.Error)
			.WithSpan(5, 35, 5, 49);
		await TestAssistants.RunAnalyzerAsync<FindNewDateTimeViaConstructorAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeIsMadeViaTargetTypeNewAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.FindNewDateTimeViaConstructorId, DiagnosticSeverity.Error)
			.WithSpan(5, 35, 5, 40);
		await TestAssistants.RunAnalyzerAsync<FindNewDateTimeViaConstructorAnalyzer>(code, [diagnostic]);
	}
}