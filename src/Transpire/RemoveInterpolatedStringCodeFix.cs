using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Transpire.Descriptors;
using System.Linq;
using System;

namespace Transpire
{
	[ExportCodeFixProvider(RemoveInterpolatedStringDescriptor.Id, LanguageNames.CSharp)]
	[Shared]
	public sealed class RemoveInterpolatedStringCodeFix
		: CodeFixProvider
	{
		public const string ChangeToLiteralStringDescription = "Change to a literal string";

		public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			if (context.CancellationToken.IsCancellationRequested)
			{
				return;
			}

			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;
			var interpolatedStringNode = (InterpolatedStringExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan);

			context.CancellationToken.ThrowIfCancellationRequested();

			var interpolatedStringNodeContent = interpolatedStringNode.Contents[0];
			var text = interpolatedStringNodeContent.GetText().ToString();

			var isVerbatim = interpolatedStringNode.DescendantTokens(_ => true).Any(_ => _.Kind() == SyntaxKind.InterpolatedVerbatimStringStartToken);

			// this works: @"@""This is a verbatim string""", "This is a verbatim string"
			//var thing = $@"@{text}";

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

		public override ImmutableArray<string> FixableDiagnosticIds =>
			ImmutableArray.Create(RemoveInterpolatedStringDescriptor.Id);
	}
}