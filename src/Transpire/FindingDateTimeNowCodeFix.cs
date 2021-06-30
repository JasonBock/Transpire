﻿using Microsoft.CodeAnalysis.CodeActions;
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
	[ExportCodeFixProvider(FindingDateTimeNowDescriptor.Id, LanguageNames.CSharp)]
	[Shared]
	public sealed class FindingDateTimeNowCodeFix
		: CodeFixProvider
	{
		public const string ChangeToUtcNowDescription = "Change to UtcNow";

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

			var nowToken = ((MemberAccessExpressionSyntax)root!.FindNode(diagnostic.Location.SourceSpan)).Name.Identifier;
			var utcNowToken = SyntaxFactory.Identifier(nowToken.LeadingTrivia,
				nameof(DateTime.UtcNow), nowToken.TrailingTrivia);

			var newRoot = root.ReplaceToken(nowToken, utcNowToken);

			context.RegisterCodeFix(
				CodeAction.Create(
					FindingDateTimeNowCodeFix.ChangeToUtcNowDescription,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)), 
					FindingDateTimeNowCodeFix.ChangeToUtcNowDescription), diagnostic);
		}

		public override ImmutableArray<string> FixableDiagnosticIds =>
			ImmutableArray.Create(FindingDateTimeNowDescriptor.Id);
	}
}