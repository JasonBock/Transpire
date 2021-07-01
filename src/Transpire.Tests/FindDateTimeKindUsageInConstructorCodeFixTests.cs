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
	public static class FindDateTimeKindUsageInConstructorCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindDateTimeKindUsageInConstructorCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id), nameof(FindDateTimeKindUsageInConstructorDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyGetFixesWhenNotUsingDateTimeKindUtcAsync()
		{
			var code =
@"using System;

public static class Test
{
  public static DateTime Make() => new DateTime(100, DateTimeKind.Local);
}";
			var document = TestAssistants.CreateDocument(code);
			var tree = await document.GetSyntaxTreeAsync();
			var compilation = (await document.Project.GetCompilationAsync())!
				.WithAnalyzers(ImmutableArray.Create((DiagnosticAnalyzer)new FindDateTimeKindUsageInConstructorAnalyzer()));
			var diagnostics = await compilation!.GetAnalyzerDiagnosticsAsync();
			var sourceSpan = diagnostics[0].Location.SourceSpan;

			var actions = new List<CodeAction>();

			var fix = new FindDateTimeKindUsageInConstructorCodeFix();
			var codeFixContext = new CodeFixContext(document, diagnostics[0],
			  (a, _) => { actions.Add(a); }, new CancellationToken(false));
			await fix.RegisterCodeFixesAsync(codeFixContext);

			Assert.Multiple(async () =>
			{
				Assert.That(actions.Count, Is.EqualTo(1), nameof(actions.Count));
				await TestAssistants.VerifyCodeFixChangesAsync(
					actions, FindDateTimeKindUsageInConstructorCodeFix.UseUtcDescription, document,
					(model, node) =>
					{
						Assert.That(node.ToString(), Contains.Substring("DateTimeKind.Utc"));
					});
			});
		}
	}
}