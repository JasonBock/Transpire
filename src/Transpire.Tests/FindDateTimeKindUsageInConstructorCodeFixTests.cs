using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;
using Verify = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.CodeFixVerifier<
	Transpire.FindDateTimeKindUsageInConstructorAnalyzer, Transpire.FindDateTimeKindUsageInConstructorCodeFix>;

namespace Transpire.Tests
{
	public static class FindDateTimeKindUsageInConstructorCodeFixTests
	{
		[Test]
		public static void VerifyGetFixableDiagnosticIds()
		{
			var fix = new FindDateTimeKindUsageInConstructorCodeFix();
			var ids = fix.FixableDiagnosticIds;

			Assert.Multiple(() =>
			{
				Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
				Assert.That(ids[0], Is.EqualTo(FindDateTimeKindUsageInConstructorDescriptor.Id), nameof(FindDateTimeKindUsageInConstructorDescriptor.Id));
			});
		}

		[Test]
		public static async Task VerifyGetFixesWhenNotUsingDateTimeKindUtcAsync()
		{
			var originalCode =
@"using System;

public static class Test
{
  public static DateTime Make() => new DateTime(100, [|DateTimeKind.Local|]);
}";
			var fixedCode =
@"using System;

public static class Test
{
  public static DateTime Make() => new DateTime(100, DateTimeKind.Utc);
}";
			await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
		}
	}
}