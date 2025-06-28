using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change <see cref="string.IsNullOrEmpty(string)"/> to <see cref="string.IsNullOrWhiteSpace(string)"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RecommendTryParseOverParseCodeFix))]
[Shared]
public sealed class RecommendIsNullOrWhiteSpaceCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string ChangeToIsNullOrWhitespaceDescription = "Change to IsNullOrWhitespace";

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
		var node = root!.FindNode(diagnostic.Location.SourceSpan);

		if (node is InvocationExpressionSyntax invocationNode)
		{
			var newInvocationNode =
				SyntaxFactory.InvocationExpression(
					SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.PredefinedType(
							SyntaxFactory.Token(SyntaxKind.StringKeyword)),
						SyntaxFactory.IdentifierName(nameof(string.IsNullOrWhiteSpace))))
					.WithArgumentList(invocationNode.ArgumentList);

			var newRoot = root.ReplaceNode(invocationNode, newInvocationNode);

			context.RegisterCodeFix(
				CodeAction.Create(
					RecommendIsNullOrWhiteSpaceCodeFix.ChangeToIsNullOrWhitespaceDescription,
					_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
					RecommendIsNullOrWhiteSpaceCodeFix.ChangeToIsNullOrWhitespaceDescription), diagnostic);
		}
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.RecommendIsNullOrWhiteSpaceId];
}