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
using Transpire.Extensions;

namespace Transpire
{
	[ExportCodeFixProvider(RecommendTryParseOverParseDescriptor.Id, LanguageNames.CSharp)]
	[Shared]
	public sealed class RecommendTryParseOverParseCodeFix
		: CodeFixProvider
	{
		public const string ChangeToTryParseDescription = "Change to TryParse";

		public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
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

			var typeName = diagnostic.Properties[RecommendTryParseOverParseDescriptor.ParameterTypeName];
			var value = diagnostic.Properties[RecommendTryParseOverParseDescriptor.ParameterValue];

			var tryParseInvocation =
				$"{typeName}.TryParse({value}, out {outType} {target});";
			var tryParseExpression =
				SyntaxFactory.ParseStatement(tryParseInvocation);

			var statementNode = node.FindParent<StatementSyntax>()!;
			var newRoot = root.ReplaceNode(statementNode, tryParseExpression.WithTriviaFrom(statementNode));

			context.RegisterCodeFix(
				CodeAction.Create(
					RecommendTryParseOverParseCodeFix.ChangeToTryParseDescription,
					_ => Task.FromResult<Document>(context.Document.WithSyntaxRoot(newRoot)),
					RecommendTryParseOverParseCodeFix.ChangeToTryParseDescription), diagnostic);
		}

		public override ImmutableArray<string> FixableDiagnosticIds =>
			ImmutableArray.Create(RecommendTryParseOverParseDescriptor.Id);
	}
}