using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change <see cref="DateTimeKind.Local"/> to <see cref="DateTimeKind.Utc"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindDateTimeKindUsageInConstructorCodeFix))]
[Shared]
public sealed class FindDateTimeKindUsageInConstructorCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string UseUtcDescription = "Use Utc";

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
		var argumentNode = (ArgumentSyntax)root!.FindNode(diagnostic.Location.SourceSpan);

		context.CancellationToken.ThrowIfCancellationRequested();

		var accessExpression = (MemberAccessExpressionSyntax)argumentNode.Expression;
		var identifierToken = accessExpression.DescendantNodes(_ => true).OfType<IdentifierNameSyntax>().Last().Identifier;
		var newIdentifierToken = SyntaxFactory.Identifier(nameof(DateTimeKind.Utc));
		var newRoot = root.ReplaceToken(identifierToken, newIdentifierToken);

		context.RegisterCodeFix(
			CodeAction.Create(
				FindDateTimeKindUsageInConstructorCodeFix.UseUtcDescription,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				FindDateTimeKindUsageInConstructorCodeFix.UseUtcDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.FindDateTimeKindUsageInConstructorId];
}