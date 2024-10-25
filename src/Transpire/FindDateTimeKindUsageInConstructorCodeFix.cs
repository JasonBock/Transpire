using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using Transpire.Descriptors;

namespace Transpire;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindDateTimeKindUsageInConstructorCodeFix))]
[Shared]
public sealed class FindDateTimeKindUsageInConstructorCodeFix
	: CodeFixProvider
{
	public const string UseUtcDescription = "Use Utc";

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
			.ConfigureAwait(false);
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

	public override ImmutableArray<string> FixableDiagnosticIds =>
		[FindDateTimeKindUsageInConstructorDescriptor.Id];
}