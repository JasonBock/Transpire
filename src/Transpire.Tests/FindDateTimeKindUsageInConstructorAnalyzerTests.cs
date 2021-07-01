using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindDateTimeKindUsageInConstructorAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindDateTimeKindUsageInConstructorAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Message),
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
			var code = "var id = 1 + 2;";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeKindIsUsedNotInDateTimeConstructorAsync()
		{
			var code = 
@"using System;

public class Usage
{
	public Usage(DateTimeKind kind) { }
}

public static class Test
{
	public static Usage Make() => new(DateTimeKind.Local);
}";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeConstructorDoesNotHaveDateTimeKindAsync()
		{
			var code = "var time = new System.DateTime(100);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeConstructorUsesDateTimeKindUtcAsync()
		{
			var code = "var time = new System.DateTime(100, System.DateTimeKind.Utc);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeConstructorDoesNotUseDateTimeKindUtcAsync()
		{
			var code = "var time = new System.DateTime(100, System.DateTimeKind.Local);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeConstructorDoesNotUseDateTimeKindUtcViaTargetTypeNewAsync()
		{
			var code = "System.DateTime time = new(100, System.DateTimeKind.Local);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindDateTimeKindUsageInConstructorAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}
	}
}