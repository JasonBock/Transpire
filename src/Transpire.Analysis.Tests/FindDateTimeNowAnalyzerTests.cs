﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class FindDateTimeNowAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindDateTimeNowAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.FindDateTimeNowId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeNowDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeNowDescriptor.Message),
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
						DescriptorIdentifiers.FindDateTimeNowId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenCallingDateTimeNowAsync()
	{
		var code =
			"""
			using System;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = DateTime.Now;
				}
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.FindDateTimeNowId, DiagnosticSeverity.Error)
			.WithSpan(7, 11, 7, 23);
		await TestAssistants.RunAnalyzerAsync<FindDateTimeNowAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenCallingDateTimeNowWithAliasAsync()
	{
		var code =
			"""
			using DT = System.DateTime;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = DT.Now;
				}
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.FindDateTimeNowId, DiagnosticSeverity.Error)
			.WithSpan(7, 11, 7, 17);
		await TestAssistants.RunAnalyzerAsync<FindDateTimeNowAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenCallingDateTimeUtcNowAsync()
	{
		var code =
			"""
			using System;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = DateTime.UtcNow;
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindDateTimeNowAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingDateTimeUtcNowWithAliasAsync()
	{
		var code =
			"""
			using DT = System.DateTime;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = DT.UtcNow;
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindDateTimeNowAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingNowAsAPropertyAsync()
	{
		var code =
			"""
			using System;

			public sealed class DateTimeTest
			{
				public void MyMethod()
				{
					var x = this.Now;
				}

				public string Now { get; set; }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindDateTimeNowAnalyzer>(code, []);
	}
}