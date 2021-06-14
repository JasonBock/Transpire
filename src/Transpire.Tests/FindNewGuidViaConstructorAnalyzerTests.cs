using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorAnalyzerTests
	{
		// AnalyzeWhenNoGuidIsMade
		// AnalyzeWhenGuidIsMadeViaGuidNewGuid
		// AnalyzeWhenGuidIsMadeViaString
		// AnalyzeWhenGuidIsMadeViaTargetTypeNew (Guid id = new())

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaNoArgumentConstructorAsync()
		{
			var code = "var id = new System.Guid();";
			var diagnostics = await FindNewGuidViaConstructorAnalyzerTests.GetDiagnosticsAsync(code);

			Assert.That(diagnostics.Length, Is.EqualTo(1));
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