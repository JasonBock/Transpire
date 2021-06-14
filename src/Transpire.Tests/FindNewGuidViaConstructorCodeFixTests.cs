using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindNewGuidViaConstructorCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), nameof(FindNewGuidViaConstructorDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
		{
			var code =
@"using System;

public static class Test
{
  public static Guid Make() => new Guid();
}";
			var document = TestAssistants.CreateDocument(code);
			var tree = await document.GetSyntaxTreeAsync();
			var compilation = (await document.Project.GetCompilationAsync())!
				.WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)new FindNewGuidViaConstructorAnalyzer()));
			var diagnostics = await compilation!.GetAnalyzerDiagnosticsAsync();
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();

			var fix = new FindNewGuidViaConstructorCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
			  (a, _) => { actions.Add(a); }, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.Multiple(async () =>
			{
				Assert.That(actions.Count, Is.EqualTo(3), nameof(actions.Count));
				await TestAssistants.VerifyCodeFixChangesAsync(
					actions, FindNewGuidViaConstructorCodeFix.AddDefaultGuidDescription, document,
					(model, node) =>
					{
						Assert.That(node.ToString(), Contains.Substring("default(Guid)"));
					});
				await TestAssistants.VerifyCodeFixChangesAsync(
					actions, FindNewGuidViaConstructorCodeFix.AddGuidEmptyDescription, document,
					(model, node) =>
					{
						Assert.That(node.ToString(), Contains.Substring("Guid.Empty"));
					});
				await TestAssistants.VerifyCodeFixChangesAsync(
					actions, FindNewGuidViaConstructorCodeFix.AddGuidNewGuidDescription, document,
					(model, node) =>
					{
						Assert.That(node.ToString(), Contains.Substring("Guid.NewGuid()"));
					});
				var guidNewGuidAction = actions.Single(_ => _.Title == FindNewGuidViaConstructorCodeFix.AddGuidNewGuidDescription);
			});
		}
	}
}