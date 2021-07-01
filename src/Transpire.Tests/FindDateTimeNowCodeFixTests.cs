using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindDateTimeNowCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindDateTimeNowCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindDateTimeNowDescriptor.Id), nameof(FindDateTimeNowDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyGetFixesWhenUsingNewGuidAsync()
		{
			var code =
@"using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DateTime.Now;
	}
}";
			var document = TestAssistants.CreateDocument(code);
			var tree = await document.GetSyntaxTreeAsync();
			var compilation = (await document.Project.GetCompilationAsync())!
				.WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)new FindDateTimeNowAnalyzer()));
			var diagnostics = await compilation!.GetAnalyzerDiagnosticsAsync();
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();

			var fix = new FindDateTimeNowCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
			  (a, _) => { actions.Add(a); }, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.Multiple(async () =>
			{
				Assert.That(actions.Count, Is.EqualTo(1), nameof(actions.Count));
				await TestAssistants.VerifyCodeFixChangesAsync(
					actions, FindDateTimeNowCodeFix.ChangeToUtcNowDescription, document,
					(model, node) =>
					{
						Assert.That(node.ToString(), Contains.Substring("DateTime.UtcNow"));
					});
			});
		}
	}
}