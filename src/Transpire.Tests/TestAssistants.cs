using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Transpire.Tests
{
	internal static class TestAssistants
	{
		internal static Document CreateDocument(string code)
		{
			var projectName = "Test";
			var projectId = ProjectId.CreateNewId(projectName);

			using var workspace = new AdhocWorkspace();
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

		internal static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync<T>(string source)
			where T : DiagnosticAnalyzer, new()
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			var compilation = CSharpCompilation.Create("analyzer", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
				.WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)new T()));

			return await compilation.GetAnalyzerDiagnosticsAsync();
		}

		internal static async Task VerifyCodeFixChangesAsync(List<CodeAction> actions, string title, Document document,
		  Action<SemanticModel, SyntaxNode> handleChanges)
		{
			var action = actions.Where(_ => _.Title == title).First();
			var operations = (await action.GetOperationsAsync(
			  new CancellationToken(false))).ToArray();
			var operation = (ApplyChangesOperation)operations[0];
			var newDoc = operation.ChangedSolution.GetDocument(document.Id);
			var newTree = await newDoc!.GetSyntaxTreeAsync();

			handleChanges((await newDoc!.GetSemanticModelAsync())!, await newTree!.GetRootAsync());
		}
	}
}