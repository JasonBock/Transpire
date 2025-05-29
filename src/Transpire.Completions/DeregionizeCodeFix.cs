using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Composition;
using System.Collections.Immutable;
using Transpire.Completions.Extensions;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to remove <c>region</c> and <c>endregion</c> directives.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DeregionizeCodeFix))]
[Shared]
public sealed class DeregionizeCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string RemoveRegionAndEndRegionDirective = "Remove #region and #endregion Directives";

	/// <summary>
	/// Gets the <see cref="FixAllProvider"/> value.
	/// </summary>
	/// <returns><see cref="WellKnownFixAllProviders.BatchFixer"/></returns>
	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <summary>
	/// Registers necessary code fixes.
	/// </summary>
	/// <param name="context">A <see cref="CodeFixContext"/> instance.</param>
	/// <returns>A <see cref="Task"/> instance.</returns>
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

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.DeregionizeId];
}
