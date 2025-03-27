using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using System.Composition;
using Transpire.Extensions;

namespace Transpire;

[ExportCodeRefactoringProvider(LanguageNames.CSharp,
	Name = nameof(DeregionizeCodeRefactoring))]
[Shared]
public sealed class DeregionizeCodeRefactoring
	: CodeRefactoringProvider
{
	public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
	{
		var document = context.Document;
		var root = await document.GetSyntaxRootAsync(context.CancellationToken);

		var newRoot = root?.Deregionize();

		if (newRoot is not null)
		{
			context.RegisterRefactoring(
				CodeAction.Create(
					"Remove #region and #endregion directives",
					token => Task.FromResult(document.WithSyntaxRoot(newRoot))
				));
		}
	}
}