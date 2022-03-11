using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = CodeFixVerifier<RemoveInterpolatedStringAnalyzer, RemoveInterpolatedStringCodeFix>;

public static class RemoveInterpolatedStringCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new RemoveInterpolatedStringCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids.Length, Is.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(RemoveInterpolatedStringDescriptor.Id), nameof(RemoveInterpolatedStringDescriptor.Id));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenInterpolatedStringHasNoInterpolationAsync()
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
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode).ConfigureAwait(false);
	}

	[Test]
	public static async Task VerifyGetFixesWhenLiteralInterpolatedStringHasNoInterpolationAsync()
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
		await Verify.VerifyCodeFixAsync(originalCode, fixedCode).ConfigureAwait(false);
	}
}