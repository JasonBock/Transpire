using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class DetectGotoUsageAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new DetectGotoUsageAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.DetectGotoUsageId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DetectGotoUsageDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DetectGotoUsageDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Warning),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.DetectGotoUsageId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenGotoDoesNotExistsAsync()
	{
		var code =
			"""
			using System;
			
			public static class Test
			{
				public static void Run() 
				{ 
					Console.WriteLine("Continue");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<DetectGotoUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenGotoStatementExistsAsync()
	{
		var code =
			"""
			using System;

			public static class Test
			{
				public static void Run() 
				{ 
					goto Done;

					Done:
					Console.WriteLine("Continue");
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DetectGotoUsageId, DiagnosticSeverity.Warning)
			.WithSpan(7, 3, 7, 13);
		await TestAssistants.RunAnalyzerAsync<DetectGotoUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenGotoCaseStatementExistsAsync()
	{
		var code =
			"""
			using System;

			public static class Test
			{
				public static void Run(int value) 
				{ 
					switch (value)
					{
						case 1:
							break;
						case 2:
							goto case 1;
					}
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DetectGotoUsageId, DiagnosticSeverity.Warning)
			.WithSpan(12, 5, 12, 17);
		await TestAssistants.RunAnalyzerAsync<DetectGotoUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenGotoDefaultStatementExistsAsync()
	{
		var code =
			"""
			using System;

			public static class Test
			{
				public static void Run(int value) 
				{ 
					switch (value)
					{
						case 1:
							break;
						case 2:
							goto default;
						default:
							Console.WriteLine("Default");
							break;
					}
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.DetectGotoUsageId, DiagnosticSeverity.Warning)
			.WithSpan(12, 5, 12, 18);
		await TestAssistants.RunAnalyzerAsync<DetectGotoUsageAnalyzer>(code, [diagnostic]);
	}
}