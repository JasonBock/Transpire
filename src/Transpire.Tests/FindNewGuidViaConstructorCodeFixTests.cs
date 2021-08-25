using Microsoft.CodeAnalysis.Testing;
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
			var test = new Test
			{
				TestCode = originalCode
			};
			test.ExpectedDiagnostics.AddRange(DiagnosticResult.EmptyDiagnosticResults);
			test.FixedCode = fixedCode;
			test.CodeActionIndex = 2;

			await test.RunAsync();
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
			var test = new Test
			{
				TestCode = originalCode
			};
			test.ExpectedDiagnostics.AddRange(DiagnosticResult.EmptyDiagnosticResults);
			test.FixedCode = fixedCode;
			test.CodeActionIndex = 1;

			await test.RunAsync();
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
			var test = new Test
			{
				TestCode = originalCode
			};
			test.ExpectedDiagnostics.AddRange(DiagnosticResult.EmptyDiagnosticResults);
			test.FixedCode = fixedCode;
			test.CodeActionIndex = 0;

			await test.RunAsync();
		}
	}
}