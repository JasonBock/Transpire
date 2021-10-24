using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Transpire.Tests
{
	using Verify = AnalyzerVerifier<RemoveInterpolatedStringAnalyzer>;

	public static class RemoveInterpolatedStringAnalyzerTests
	{
		[Test]
		public static async Task AnalyzeWhenInterpolatedStringHasNoInterpolations()
		{
			var code =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = [|$""This has no interpolations.""|];
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenInterpolatedStringHasInterpolations()
		{
			var code =
@"using System;

public sealed class StringTest
{
	public void MyMethod(int value)
	{
		var x = $""This has an interpolation: {value}."";
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenStringIsLiteral()
		{
			var code =
@"using System;

public sealed class StringTest
{
	public void MyMethod()
	{
		var x = ""This is a literal string."";
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}
	}
}
