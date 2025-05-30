﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

using Verify = CSharpAnalyzerVerifier<FindNewGuidViaConstructorAnalyzer, DefaultVerifier>;

internal static class FindNewGuidViaConstructorAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindNewGuidViaConstructorAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.FindNewGuidViaConstructorId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Message),
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
						DescriptorIdentifiers.FindNewGuidViaConstructorId, 
						FindNewGuidViaConstructorDescriptor.Title)),
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
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenNoGuidIsMadeAsync()
	{
		var code =
			"""
			internal static class Test
			{
				public static string Make() => new string('a', 1);
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenGuidIsMadeViaGuidNewGuidAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => Guid.NewGuid();
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenGuidIsMadeViaStringAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => new Guid("83d926c8-9fe6-4cd2-8495-e294e8ade4cb");
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenGuidIsMadeViaNoArgumentConstructorAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new Guid()|];
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenGuidIsMadeViaTargetTypeNewAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static Guid Make() => [|new()|];
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}
}