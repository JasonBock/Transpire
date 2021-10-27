using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Transpire.Tests
{
	using Verify = CodeFixVerifier<RemoveInterpolatedStringAnalyzer, RemoveInterpolatedStringCodeFix>;

	public static class RemoveInterpolatedStringCodeFixTests
	{
		[Test]
		public static async Task VerifyGetFixesWhenInterpolatedStringHasNoInterpolation()
		{
			var originalCode =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = [|$""This has no interpolations.""|];
	}
}";
			var fixedCode =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = ""This has no interpolations."";
	}
}";
			await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
		}

		[Test]
		public static async Task VerifyGetFixesWhenLiteralInterpolatedStringHasNoInterpolation()
		{
			var originalCode =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = 
[|$@""this is
a verbatim string.""|];
	}
}";
			var fixedCode =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = 
@""this is
a verbatim string."";
	}
}";
			await Verify.VerifyCodeFixAsync(originalCode, fixedCode);
		}
	}
}
