using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Globalization;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = AnalyzerVerifier<RemoveInterpolatedStringAnalyzer>;

public static class RemoveInterpolatedStringAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new RemoveInterpolatedStringAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(RemoveInterpolatedStringDescriptor.Id),
					 nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RemoveInterpolatedStringDescriptor.Title),
					 nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RemoveInterpolatedStringDescriptor.Message),
					 nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					 nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Info),
					 nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					 nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(RemoveInterpolatedStringDescriptor.Id, RemoveInterpolatedStringDescriptor.Title)),
					 nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenInterpolatedStringHasNoInterpolationsAsync()
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
		await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
	}

	[Test]
	public static async Task AnalyzeWhenInterpolatedStringHasInterpolationsAsync()
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
		await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
	}

	[Test]
	public static async Task AnalyzeWhenStringIsLiteralAsync()
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
		await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
	}
}