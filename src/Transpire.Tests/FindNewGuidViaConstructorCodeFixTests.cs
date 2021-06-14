using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Threading;

namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorCodeFixTests
	{
		[Test]
		public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
		{
			var code =
@"using System;

public static class Test
{
  public static Guid Make() => new Guid();
}";
			var document = FindNewGuidViaConstructorCodeFixTests.Create(code);
			var tree = await document.GetSyntaxTreeAsync();
			var compilation = (await document.Project.GetCompilationAsync())!
				.WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)new FindNewGuidViaConstructorAnalyzer()));
			var diagnostics = await compilation!.GetAnalyzerDiagnosticsAsync();
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();
			var codeActionRegistration = new Action<CodeAction, ImmutableArray<Diagnostic>>(
			  (a, _) => { actions.Add(a); });

			var fix = new FindNewGuidViaConstructorCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
			  codeActionRegistration, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.That(actions.Count, Is.EqualTo(3), nameof(actions.Count));
		}

		private static Document Create(string code)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			using (var workspace = new AdhocWorkspace())
			{
				var references = AppDomain.CurrentDomain.GetAssemblies()
					.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
					.Select(_ => MetadataReference.CreateFromFile(_.Location));

				var solution = workspace.CurrentSolution
				  .AddProject(projectId, projectName, projectName, LanguageNames.CSharp)
				  .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				  .AddMetadataReferences(projectId, references);

				var documentId = DocumentId.CreateNewId(projectId);
				solution = solution.AddDocument(documentId, "Test.cs", SourceText.From(code));

				return solution.GetProject(projectId)!.Documents.First();
			}
		}
	}
}