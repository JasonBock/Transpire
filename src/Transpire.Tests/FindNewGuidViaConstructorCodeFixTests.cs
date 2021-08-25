using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;
using Test = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixTest<
	Transpire.FindNewGuidViaConstructorAnalyzer, Transpire.FindNewGuidViaConstructorCodeFix, Microsoft.CodeAnalysis.Testing.Verifiers.NUnitVerifier>;
namespace Transpire.Tests
{
	public static class FindNewGuidViaConstructorCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindNewGuidViaConstructorCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), nameof(FindNewGuidViaConstructorDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyDefaultGuidCodeFixAsync()
		{
			var originalCode =
@"using System;

public static class Test
{
  public static Guid Make() => [|new Guid()|];
}";
			var fixedCode =
@"using System;

public static class Test
{
  public static Guid Make() => default(Guid);
}";
			await FindNewGuidViaConstructorCodeFixTests.Verify(originalCode, fixedCode, 2);
		}

		[Test]
		public static async Task VerifyGuidEmptyCodeFixAsync()
		{
			var originalCode =
@"using System;

public static class Test
{
  public static Guid Make() => [|new Guid()|];
}";
			var fixedCode =
@"using System;

public static class Test
{
  public static Guid Make() => Guid.Empty;
}";
			await FindNewGuidViaConstructorCodeFixTests.Verify(originalCode, fixedCode, 1);
		}

		[Test]
		public static async Task VerifyGuidNewGuidCodeFixAsync()
		{
			var originalCode =
@"using System;

public static class Test
{
  public static Guid Make() => [|new Guid()|];
}";
			var fixedCode =
@"using System;

public static class Test
{
  public static Guid Make() => Guid.NewGuid();
}";
			await FindNewGuidViaConstructorCodeFixTests.Verify(originalCode, fixedCode, 0);
		}

		private static async Task Verify(string originalCode, string fixedCode, int codeActionIndex)
		{
			var test = new Test
			{
				TestCode = originalCode
			};
			test.FixedCode = fixedCode;
			test.CodeActionIndex = codeActionIndex;

			await test.RunAsync();
		}
	}
}