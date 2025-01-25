using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = CSharpAnalyzerVerifier<FindDateTimeKindUsageInConstructorAnalyzer, DefaultVerifier>;

internal static class FindDateTimeKindUsageInConstructorAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindDateTimeKindUsageInConstructorAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindDateTimeKindUsageInConstructorDescriptor.Id, FindDateTimeKindUsageInConstructorDescriptor.Title)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenNothingIsMadeAsync()
	{
		var code =
			"""
			public sealed class Usage { }

			internal static class Test
			{
				public static Usage Make() => new Usage();
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeKindIsUsedNotInDateTimeConstructorAsync()
	{
		var code =
			"""
			using System;

			public class Usage
			{
				public Usage(DateTimeKind kind) { }
			}

			internal static class Test
			{
				public static Usage Make() => new Usage(DateTimeKind.Local);
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeConstructorDoesNotHaveDateTimeKindAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100);
			}
			""";

		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeConstructorUsesDateTimeKindUtcAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100, DateTimeKind.Utc);
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeConstructorDoesNotUseDateTimeKindUtcAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new DateTime(100, [|DateTimeKind.Local|]);
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}

	[Test]
	public static async Task AnalyzeWhenDateTimeConstructorDoesNotUseDateTimeKindUtcViaTargetTypeNewAsync()
	{
		var code =
			"""
			using System;

			internal static class Test
			{
				public static DateTime Make() => new(100, [|DateTimeKind.Local|]);
			}
			""";
		await Verify.VerifyAnalyzerAsync(code);
	}
}