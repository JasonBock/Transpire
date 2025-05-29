using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change <see cref="DateTime.Now"/> to <see cref="DateTime.UtcNow"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindDateTimeNowCodeFix))]
[Shared]
public sealed class FindDateTimeNowCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string ChangeToUtcNowDescription = "Change to UtcNow";

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
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
		var diagnostic = context.Diagnostics.First();
		var diagnosticSpan = diagnostic.Location.SourceSpan;

		context.CancellationToken.ThrowIfCancellationRequested();

		var nowToken = ((MemberAccessExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan)).Name.Identifier;
		var utcNowToken = SyntaxFactory.Identifier(nowToken.LeadingTrivia,
			nameof(DateTime.UtcNow), nowToken.TrailingTrivia);

		var newRoot = root.ReplaceToken(nowToken, utcNowToken);

		context.RegisterCodeFix(
			CodeAction.Create(
				FindDateTimeNowCodeFix.ChangeToUtcNowDescription,
				_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)),
				FindDateTimeNowCodeFix.ChangeToUtcNowDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.FindDateTimeNowId];
}