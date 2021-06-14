using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindNewGuidViaConstructorAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];
				
				Assert.That(diagnostic.Id, Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), 
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Title), 
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Message), 
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage), 
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), 
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindNewGuidViaConstructorDescriptor.Id, FindNewGuidViaConstructorDescriptor.Title)), 
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenNothingIsMadeAsync()
		{
			var code = "var id = 1 + 2;";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenNoGuidIsMadeAsync()
		{
			var code = "var id = new string('a', 1);";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaGuidNewGuidAsync()
		{
			var code = "var id = System.Guid.NewGuid();";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaStringAsync()
		{
			var code = "var id = new System.Guid(\"83d926c8-9fe6-4cd2-8495-e294e8ade4cb\");";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaNoArgumentConstructorAsync()
		{
			var code = "var id = new System.Guid();";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaTargetTypeNewAsync()
		{
			var code = "System.Guid id = new();";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}
	}
}