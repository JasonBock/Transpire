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
/// Defines a code fix to change <see cref="Guid()"/> to <see cref="Guid.NewGuid()" />
/// or <c>Guid.CreateVersion7()</c> if it's available./>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewGuidViaConstructorCodeFix))]
[Shared]
public sealed class FindNewGuidViaConstructorCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title for "add default".
	/// </summary>
	public const string AddDefaultGuidDescription = "Add default(Guid)";
	/// <summary>
	/// Specifies the code fix title for "add Guid.Empty".
	/// </summary>
	public const string AddGuidEmptyDescription = "Add Guid.Empty";
	/// <summary>
	/// Specifies the code fix title for "add Guid.NewGuid()".
	/// </summary>
	public const string AddGuidNewGuidDescription = "Add Guid.NewGuid()";
	/// <summary>
	/// Specifies the code fix title for "add Guid.CreateVersion7()".
	/// </summary>
	public const string AddGuidCreateVersion7Description = "Add Guid.CreateVersion7()";

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
		var creationNode = root!.FindNode(diagnostic.Location.SourceSpan);

		context.CancellationToken.ThrowIfCancellationRequested();

		FindNewGuidViaConstructorCodeFix.AddGuidNewGuidCodeFix(context, root, diagnostic, creationNode);
		FindNewGuidViaConstructorCodeFix.AddGuidEmptyCodeFix(context, root, diagnostic, creationNode);
		FindNewGuidViaConstructorCodeFix.AddDefaultCodeFix(context, root, diagnostic, creationNode);

		if (bool.Parse(diagnostic.Properties[Constants.DoesCreateVersion7ExistKey]))
		{
			FindNewGuidViaConstructorCodeFix.AddGuidCreateVersion7CodeFix(context, root, diagnostic, creationNode);
		}
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

	private static void AddGuidCreateVersion7CodeFix(CodeFixContext context, SyntaxNode root,
		Diagnostic diagnostic, SyntaxNode creationNode)
	{
		var newInvocationNode = SyntaxFactory.InvocationExpression(
			SyntaxFactory.MemberAccessExpression(
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxFactory.IdentifierName(nameof(Guid)),
				SyntaxFactory.IdentifierName(Constants.CreateVersion7MemberName)))
			.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
		FindNewGuidViaConstructorCodeFix.AddCodeFix(context, root, diagnostic,
			creationNode, newInvocationNode, FindNewGuidViaConstructorCodeFix.AddGuidCreateVersion7Description);
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

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.FindNewGuidViaConstructorId];
}