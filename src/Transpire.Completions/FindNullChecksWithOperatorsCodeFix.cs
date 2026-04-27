using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change null checks using <c>==</c> or <c>!=</c> to an is pattern.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNullChecksWithOperatorsCodeFix))]
[Shared]
public sealed class FindNullChecksWithOperatorsCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string UseIsPatternDescription = "Use is pattern";

	/// <summary>
	/// Gets the <see cref="FixAllProvider"/> value.
	/// </summary>
	/// <returns><see cref="WellKnownFixAllProviders.BatchFixer"/></returns>
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <summary>
	/// Registers necessary code fixes.
	/// </summary>
	/// <param name="context">A <see cref="CodeFixContext"/> instance.</param>
	/// <returns>A <see cref="Task"/> instance.</returns>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
		var diagnostic = context.Diagnostics.First();
		var binaryNode = (BinaryExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan);

		context.CancellationToken.ThrowIfCancellationRequested();

		var pattern = binaryNode.Kind() == SyntaxKind.EqualsExpression ? "is" : "is not";
		var patternExpression = SyntaxFactory.ParseExpression($"{binaryNode.Left} {pattern} {binaryNode.Right}")
			.WithLeadingTrivia(binaryNode.GetLeadingTrivia())
			.WithTrailingTrivia(binaryNode.GetTrailingTrivia());
		var newRoot = root.ReplaceNode(binaryNode, patternExpression);

		context.RegisterCodeFix(
			CodeAction.Create(
				FindNullChecksWithOperatorsCodeFix.UseIsPatternDescription,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				FindNullChecksWithOperatorsCodeFix.UseIsPatternDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.FindNullChecksWithOperatorsId];
}