using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindNewDateTimeViaConstructorAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindNewDateTimeViaConstructorAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Message),
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindNewDateTimeViaConstructorDescriptor.Id, FindNewDateTimeViaConstructorDescriptor.Title)),
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenNothingIsMadeAsync()
		{
			var code = "var id = 1 + 2;";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindNewDateTimeViaConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenNoDateIsMadeAsync()
		{
			var code = "var id = new string('a', 1);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindNewDateTimeViaConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaParameters()
		{
			var code = "var id = new System.DateTime(100, System.DateTimeKind.Local);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindNewDateTimeViaConstructorAnalyzer>(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaNoArgumentConstructorAsync()
		{
			var code = "var id = new System.DateTime();";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindNewDateTimeViaConstructorAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaTargetTypeNewAsync()
		{
			var code = "System.DateTime id = new();";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindNewDateTimeViaConstructorAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}
	}
}