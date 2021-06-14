using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorAnalyzerTests
	{
		[Test]
		public static async Task AnalyzeWhenNothingIsMadeAsync()
		{
			var code = "var id = 1 + 2;";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenNoGuidIsMadeAsync()
		{
			var code = "var id = new string('a', 1);";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaGuidNewGuidAsync()
		{
			var code = "var id = System.Guid.NewGuid();";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaStringAsync()
		{
			var code = "var id = new System.Guid(\"83d926c8-9fe6-4cd2-8495-e294e8ade4cb\");";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaNoArgumentConstructorAsync()
		{
			var code = "var id = new System.Guid();";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(CallingNewGuidDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(CallingNewGuidDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaTargetTypeNewAsync()
		{
			var code = "System.Guid id = new();";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(CallingNewGuidDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(CallingNewGuidDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("analyzer", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				.WithAnalyzers(ImmutableArray.Create(new FindNewGuidViaConstructorAnalyzer() as DiagnosticAnalyzer));

			return await compilation.GetAnalyzerDiagnosticsAsync();
		}
	}
}