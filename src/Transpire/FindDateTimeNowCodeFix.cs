using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;
using Transpire.Descriptors;

namespace Transpire;

[ExportCodeFixProvider(FindDateTimeNowDescriptor.Id, LanguageNames.CSharp)]
[Shared]
public sealed class FindDateTimeNowCodeFix
	 : CodeFixProvider
{
	public const string ChangeToUtcNowDescription = "Change to UtcNow";

	public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
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

	public override ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(FindDateTimeNowDescriptor.Id);
}