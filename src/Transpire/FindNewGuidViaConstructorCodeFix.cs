using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Transpire.Descriptors;
using Transpire.Extensions;

namespace Transpire
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewGuidViaConstructorCodeFix))]
	[Shared]
	public sealed class FindNewGuidViaConstructorCodeFix
		: CodeFixProvider
	{
		public const string AddGuidNewGuidDescription = "Add Guid.NewGuid()";

		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CallingNewGuidDescriptor.Id);

		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			context.CancellationToken.ThrowIfCancellationRequested();

			var diagnostic = context.Diagnostics.First();
			var creationNode = (ObjectCreationExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan);
			FindNewGuidViaConstructorCodeFix.AddGuidNewGuidCodeFix(context, root, diagnostic, creationNode);
		}

		private static void AddGuidNewGuidCodeFix(CodeFixContext context, SyntaxNode root,
			Diagnostic diagnostic, ObjectCreationExpressionSyntax creationNode)
		{
			var newInvocationNode = SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.IdentifierName(nameof(Guid)),
						SyntaxFactory.IdentifierName(nameof(Guid.NewGuid))))
				.NormalizeWhitespace().WithAdditionalAnnotations(Formatter.Annotation);
			var newRoot = root.ReplaceNode(creationNode, newInvocationNode);

			var guidNamespace = typeof(Guid).Namespace;

			if (!root.HasUsing(guidNamespace))
			{
				newRoot = ((CompilationUnitSyntax)newRoot).AddUsings(
				  SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(guidNamespace)));
			}

			context.RegisterCodeFix(
			  CodeAction.Create(
				 FindNewGuidViaConstructorCodeFix.AddGuidNewGuidDescription,
				 _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				 FindNewGuidViaConstructorCodeFix.AddGuidNewGuidDescription), diagnostic);
		}
	}
}