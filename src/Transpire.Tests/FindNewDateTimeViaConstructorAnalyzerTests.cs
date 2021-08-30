using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	using Verify = AnalyzerVerifier<FindNewDateTimeViaConstructorAnalyzer>;

	public static class FindNewDateTimeViaConstructorAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindNewDateTimeViaConstructorAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(FindNewDateTimeViaConstructorDescriptor.Message),
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindNewDateTimeViaConstructorDescriptor.Id, FindNewDateTimeViaConstructorDescriptor.Title)),
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenNothingIsMadeAsync()
		{
			var code =
@"public static class Test
{
	public static int Make() => 1 + 2;
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenNoDateIsMadeAsync()
		{
			var code =
@"public static class Test
{
	public static string Make() => new string('a', 1);
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaParameters()
		{
			var code =
@"using System;

public static class Test
{
	public static DateTime Make() => new DateTime(100, DateTimeKind.Local);
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaNoArgumentConstructorAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static DateTime Make() => [|new DateTime()|];
}";
			await Verify.VerifyAnalyzerAsync(code);
		}

		[Test]
		public static async Task AnalyzeWhenDateTimeIsMadeViaTargetTypeNewAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static DateTime Make() => [|new()|];
}";
			await Verify.VerifyAnalyzerAsync(code);
		}
	}
}