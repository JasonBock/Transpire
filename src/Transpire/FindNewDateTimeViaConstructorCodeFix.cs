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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewDateTimeViaConstructorCodeFix))]
[Shared]
public sealed class FindNewDateTimeViaConstructorCodeFix
	 : CodeFixProvider
{
	public const string AddDateTimeUtcNowDescription = "Add DateTime.UtcNow";

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
			.ConfigureAwait(false);
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

	public override ImmutableArray<string> FixableDiagnosticIds =>
		ImmutableArray.Create(FindNewDateTimeViaConstructorDescriptor.Id);
}