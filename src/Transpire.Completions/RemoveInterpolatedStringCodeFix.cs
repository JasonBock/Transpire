using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to remove <c>$</c> from a non-interpolated string.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveInterpolatedStringCodeFix))]
[Shared]
public sealed class RemoveInterpolatedStringCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string ChangeToLiteralStringDescription = "Change to a literal string";

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

		var interpolatedStringNode = (InterpolatedStringExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan);
		var interpolatedStringNodeContent = interpolatedStringNode.Contents[0];
		var text = interpolatedStringNodeContent.GetText().ToString();
		var isVerbatim = interpolatedStringNode.DescendantTokens(_ => true).Any(_ => _.IsKind(SyntaxKind.InterpolatedVerbatimStringStartToken));

		var stringNode = isVerbatim ?
			SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
				SyntaxFactory.Literal($@"@""{text}""", text))
				.WithLeadingTrivia(interpolatedStringNodeContent.GetLeadingTrivia())
				.WithTrailingTrivia(interpolatedStringNodeContent.GetTrailingTrivia()) :
			SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
				SyntaxFactory.Literal(text))
				.WithLeadingTrivia(interpolatedStringNodeContent.GetLeadingTrivia())
				.WithTrailingTrivia(interpolatedStringNodeContent.GetTrailingTrivia());

		var newRoot = root.ReplaceNode(interpolatedStringNode, stringNode);

		context.RegisterCodeFix(
			CodeAction.Create(
				RemoveInterpolatedStringCodeFix.ChangeToLiteralStringDescription,
				_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)),
				RemoveInterpolatedStringCodeFix.ChangeToLiteralStringDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.RemoveInterpolatedStringId];
}