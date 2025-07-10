using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class DeregionizerAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new DeregionizeAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.DeregionizeId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DeregionizeDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(DeregionizeDescriptor.Message),
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
						DescriptorIdentifiers.DeregionizeId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenRegionEndRegionDirectivesDoNotExistAsync()
	{
		var code =
			"""
			using System;

			public interface IAmUseless { }
			""";

		await TestAssistants.RunAnalyzerAsync<DeregionizeAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenRegionEndRegionDirectivesExistsAsync()
	{
		var code =
			"""
			using System;

			#region Useless code
			public interface IAmUseless { }
			#endregion
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.DeregionizeId, DiagnosticSeverity.Info)
			.WithSpan(1, 1, 5, 11);
		await TestAssistants.RunAnalyzerAsync<DeregionizeAnalyzer>(code, [diagnostic]);
	}
}