using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to add digit separators in a numeric literal expression.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DetectNonSeparatedDigitsCodeFix))]
[Shared]
public sealed class DetectNonSeparatedDigitsCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title for 2 characters.
	/// </summary>
	public const string AddDigitSeparators2Characters = "Add digit separators every 2 characters";
	/// <summary>
	/// Specifies the code fix title for 3 characters.
	/// </summary>
	public const string AddDigitSeparators3Characters = "Add digit separators every 3 characters";
	/// <summary>
	/// Specifies the code fix title for 4 characters.
	/// </summary>
	public const string AddDigitSeparators4Characters = "Add digit separators every 4 characters";

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

		var literalNode = (LiteralExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan);
		var literalInformation = new LiteralNumberInformation(literalNode);

		if (literalInformation.Prefix != string.Empty)
		{
			// Offer 2 and 4 spacing
			DetectNonSeparatedDigitsCodeFix.OfferSpacedLiteral(
				context, root, diagnostic,
				literalInformation.CreateSeparated(2).ToString(), literalNode,
				DetectNonSeparatedDigitsCodeFix.AddDigitSeparators2Characters);
			DetectNonSeparatedDigitsCodeFix.OfferSpacedLiteral(
				context, root, diagnostic,
				literalInformation.CreateSeparated(4).ToString(), literalNode,
				DetectNonSeparatedDigitsCodeFix.AddDigitSeparators4Characters);
		}
		else
		{
			// Offer 3 spacing.
			DetectNonSeparatedDigitsCodeFix.OfferSpacedLiteral(
				context, root, diagnostic,
				literalInformation.CreateSeparated(3).ToString(), literalNode,
				DetectNonSeparatedDigitsCodeFix.AddDigitSeparators3Characters);
		}
	}

	private static void OfferSpacedLiteral(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, string newLiteralText,
		LiteralExpressionSyntax literalNode, string title)
	{
		var newLiteralToken = SyntaxFactory.ParseToken(newLiteralText);
		var newLiteralNode = SyntaxFactory.LiteralExpression(
			SyntaxKind.NumericLiteralExpression, newLiteralToken);
		var newRoot = root.ReplaceNode(literalNode, newLiteralNode);

		context.RegisterCodeFix(
			CodeAction.Create(
				title,
				_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)),
				title), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.DetectNonSeparatedDigitsId];
}