using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Composition;
using Transpire.Descriptors;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using Transpire.Extensions;

namespace Transpire;

[ExportCodeFixProvider(FindDateTimeNowDescriptor.Id, LanguageNames.CSharp)]
[Shared]
public sealed class DeregionizeCodeFix
	: CodeFixProvider
{
	public const string RemoveRegionAndEndRegionDirective = "Remove #region and #endregion Directives";

	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = (await context.Document.GetSyntaxRootAsync(context.CancellationToken))!;
		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		context.CancellationToken.ThrowIfCancellationRequested();

		var newRoot = root.Deregionize();

		context.RegisterCodeFix(
			CodeAction.Create(
				DeregionizeCodeFix.RemoveRegionAndEndRegionDirective,
				_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)),
				DeregionizeCodeFix.RemoveRegionAndEndRegionDirective), diagnostic);
	}

	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DeregionizeDescriptor.Id];
}
