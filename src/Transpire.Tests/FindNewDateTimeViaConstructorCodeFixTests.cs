using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.CodeFixVerifier<
	Transpire.FindNewDateTimeViaConstructorAnalyzer, Transpire.FindNewDateTimeViaConstructorCodeFix>;

namespace Transpire.Tests
{
	public static class FindNewDateTimeViaConstructorCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindNewDateTimeViaConstructorCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id), nameof(FindNewDateTimeViaConstructorDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyGetFixesWhenUsingNewDateTimeAsync()
		{
			var originalCode =
@"using System;

public static class Test
{
  public static DateTime Make() => [|new DateTime()|];
}";
			var fixedCode =
@"using System;

public static class Test
{
  public static DateTime Make() => DateTime.UtcNow;
}";
			await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
		}
	}
}