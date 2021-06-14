using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FindNewGuidViaConstructorCodeFix))]
	[Shared]
	public sealed class FindNewGuidViaConstructorCodeFix
		: CodeFixProvider
	{
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CallingNewGuidDescriptor.Id);

		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			context.CancellationToken.ThrowIfCancellationRequested();

			var diagnostic = context.Diagnostics.First();
			var creationNode = root!.FindNode(diagnostic.Location.SourceSpan) as ObjectCreationExpressionSyntax;
		}
	}

	public struct Thingee
	{
		private readonly int x;

		public Thingee() => this.x = 3;
	}
}