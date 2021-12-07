using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using Transpire.Descriptors;
using Transpire.Extensions;

namespace Transpire;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewGuidViaConstructorCodeFix))]
[Shared]
public sealed class FindNewGuidViaConstructorCodeFix
	: CodeFixProvider
{
	public const string AddDefaultGuidDescription = "Add default(Guid)";
	public const string AddGuidEmptyDescription = "Add Guid.Empty";
	public const string AddGuidNewGuidDescription = "Add Guid.NewGuid()";

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
			.ConfigureAwait(false);
		var diagnostic = context.Diagnostics.First();
		var creationNode = root!.FindNode(diagnostic.Location.SourceSpan);

		context.CancellationToken.ThrowIfCancellationRequested();

		FindNewGuidViaConstructorCodeFix.AddGuidNewGuidCodeFix(context, root, diagnostic, creationNode);
		FindNewGuidViaConstructorCodeFix.AddGuidEmptyCodeFix(context, root, diagnostic, creationNode);
		FindNewGuidViaConstructorCodeFix.AddDefaultCodeFix(context, root, diagnostic, creationNode);
	}

	private static void AddCodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode, SyntaxNode newNode,
		string description)
	{
		var newRoot = root.ReplaceNode(creationNode, newNode);

		var guidNamespace = typeof(Guid).Namespace;

		if (!root.HasUsing(guidNamespace))
		{
			newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(
				SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(guidNamespace)));
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				description, _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				description), diagnostic);
	}

	private static void AddGuidNewGuidCodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode)
	{
		var newInvocationNode = SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName(nameof(Guid)),
				SyntaxFactory.IdentifierName(nameof(Guid.NewGuid))))
			.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
		FindNewGuidViaConstructorCodeFix.AddCodeFix(context, root, diagnostic,
			creationNode, newInvocationNode, FindNewGuidViaConstructorCodeFix.AddGuidNewGuidDescription);
	}

	private static void AddGuidEmptyCodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode)
	{
		var newAccessExpressionNode = SyntaxFactory.MemberAccessExpression(
			SyntaxKind.SimpleMemberAccessExpression,
			SyntaxFactory.IdentifierName(nameof(Guid)),
			SyntaxFactory.IdentifierName(nameof(Guid.Empty)))
			.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
		FindNewGuidViaConstructorCodeFix.AddCodeFix(context, root, diagnostic,
			creationNode, newAccessExpressionNode, FindNewGuidViaConstructorCodeFix.AddGuidEmptyDescription);
	}

	private static void AddDefaultCodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode)
	{
		var defaultExpressionNode = SyntaxFactory.DefaultExpression(
			SyntaxFactory.IdentifierName(nameof(Guid)))
			.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
		FindNewGuidViaConstructorCodeFix.AddCodeFix(context, root, diagnostic,
			creationNode, defaultExpressionNode, FindNewGuidViaConstructorCodeFix.AddDefaultGuidDescription);
	}

	public override ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(FindNewGuidViaConstructorDescriptor.Id);
}