using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using Transpire.Completions.Extensions;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change <see cref="DateTime()"/> to <see cref="DateTime.UtcNow"/>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewDateTimeViaConstructorCodeFix))]
[Shared]
public sealed class FindNewDateTimeViaConstructorCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string AddDateTimeUtcNowDescription = "Add DateTime.UtcNow";

	/// <summary>
	/// Gets the <see cref="FixAllProvider"/> value.
	/// </summary>
	/// <returns><see cref="WellKnownFixAllProviders.BatchFixer"/></returns>
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	/// <summary>
	/// Registers necessary code fixes.
	/// </summary>
	/// <param name="context">A <see cref="CodeFixContext"/> instance.</param>
	/// <returns></returns>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
			;
		var diagnostic = context.Diagnostics.First();
		var creationNode = root!.FindNode(diagnostic.Location.SourceSpan);

		context.CancellationToken.ThrowIfCancellationRequested();

		FindNewDateTimeViaConstructorCodeFix.AddDateTimeUtcNowCodeFix(context, root, diagnostic, creationNode);
	}

	private static void AddDateTimeUtcNowCodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode)
	{
		var newAccessExpressionNode = SyntaxFactory.MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			SyntaxFactory.IdentifierName(nameof(DateTime)),
			SyntaxFactory.IdentifierName(nameof(DateTime.UtcNow)))
			.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);

		var newRoot = root.ReplaceNode(creationNode, newAccessExpressionNode);

		var dateTimeNamespace = typeof(DateTime).Namespace;

		if (!root.HasUsing(dateTimeNamespace))
		{
			newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(
			  SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(dateTimeNamespace)));
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				FindNewDateTimeViaConstructorCodeFix.AddDateTimeUtcNowDescription,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				FindNewDateTimeViaConstructorCodeFix.AddDateTimeUtcNowDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.FindNewDateTimeViaConstructorId];
}