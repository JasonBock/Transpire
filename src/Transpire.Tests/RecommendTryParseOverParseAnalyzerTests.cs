using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	using Verify = AnalyzerVerifier<RecommendTryParseOverParseAnalyzer>;

	public static class RecommendTryParseOverParseAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new RecommendTryParseOverParseAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(RecommendTryParseOverParseDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(RecommendTryParseOverParseDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(RecommendTryParseOverParseDescriptor.Message),
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(RecommendTryParseOverParseDescriptor.Id, RecommendTryParseOverParseDescriptor.Title)),
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenCallingParse()
		{
			var code =
@"using System;

public sealed class IntParseTest
{
	public void MyMethod()
	{
		var x = [|int.Parse(""3"")|];
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenCallingParseWithReturnTypeAndContainingTypeDoNotMatch()
		{
			var code =
@"using System;

public static class MyParser
{
	public static int Parse(string value) => 0;
}

public sealed class IntParseTest
{
	public void MyMethod()
	{
		var x = MyParser.Parse(""3"");
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenCallingParseAsInstanceMethod()
		{
			var code =
@"using System;

public class MyParser
{
	public MyParser Parse(string value) => new MyParser();
}

public sealed class IntParseTest
{
	public void MyMethod()
	{
		var x = new MyParser().Parse(""3"");
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenNotCallingParse()
		{
			var code =
@"using System;

public sealed class IntParseTest
{
	public void MyMethod()
	{
		int.TryParse(""3"", out var x);
	}
}";
			await Verify.VerifyAnalyzerAsync(code);
		}
	}
}