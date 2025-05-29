using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

using Verify = CSharpAnalyzerVerifier<RemoveInterpolatedStringAnalyzer, DefaultVerifier>;

internal static class RemoveInterpolatedStringAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new RemoveInterpolatedStringAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.RemoveInterpolatedStringId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RemoveInterpolatedStringDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RemoveInterpolatedStringDescriptor.Message),
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
						DescriptorIdentifiers.RemoveInterpolatedStringId, 
						RemoveInterpolatedStringDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenInterpolatedStringHasNoInterpolationsAsync()
	{
		var code =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = [|$"This has no interpolations."|];
				}
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenInterpolatedStringHasInterpolationsAsync()
	{
		var code =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod(int value)
				{
					var x = $"This has an interpolation: {value}.";
				}
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenStringIsLiteralAsync()
	{
		var code =
			"""
			using System;

			public sealed class StringTest
			{
				public void MyMethod()
				{
					var x = "This is a literal string.";
				}
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}
}