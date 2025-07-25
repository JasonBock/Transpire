﻿using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;
using Transpire.Completions.Extensions;

namespace Transpire.Completions;

/// <summary>
/// Defines a code fix to change <c>Parse()</c> to <c>TryParse()</c>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RecommendTryParseOverParseCodeFix))]
[Shared]
public sealed class RecommendTryParseOverParseCodeFix
	: CodeFixProvider
{
	/// <summary>
	/// Specifies the code fix title.
	/// </summary>
	public const string ChangeToTryParseDescription = "Change to TryParse";

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

		context.CancellationToken.ThrowIfCancellationRequested();

		var localDeclarationNode = node.FindParent<LocalDeclarationStatementSyntax>();
		var outType = string.Empty;
		var target = string.Empty;

		if (localDeclarationNode is not null)
		{
			outType = localDeclarationNode.Declaration.Type.IsVar ?
				"var" : localDeclarationNode.Declaration.Type.GetText().ToString();
			// TODO: I'm assuming one assignment, there might be more...
			target = localDeclarationNode.Declaration.Variables[0].Identifier.Text;
		}
		else
		{
			var assignmentNode = node.FindParent<AssignmentExpressionSyntax>();

			if (assignmentNode is not null)
			{
				target = assignmentNode.Left.GetText().ToString();
			}
		}

		var typeName = diagnostic.Properties[Constants.RecommendTryParseOverParseParameterTypeName];
		var value = diagnostic.Properties[Constants.RecommendTryParseOverParseParameterValue];

		var tryParseInvocation =
			$"{typeName}.TryParse({value}, out {outType} {target});";
		var tryParseExpression =
			SyntaxFactory.ParseStatement(tryParseInvocation);

		var statementNode = node.FindParent<StatementSyntax>()!;
		var newRoot = root.ReplaceNode(statementNode, tryParseExpression.WithTriviaFrom(statementNode));

		context.RegisterCodeFix(
			CodeAction.Create(
				RecommendTryParseOverParseCodeFix.ChangeToTryParseDescription,
				_ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
				RecommendTryParseOverParseCodeFix.ChangeToTryParseDescription), diagnostic);
	}

	/// <summary>
	/// Gets a list of diagnostic identifiers that this code fixer can address.
	/// </summary>
	public override ImmutableArray<string> FixableDiagnosticIds =>
		[DescriptorIdentifiers.RecommendTryParseOverParseId];
}