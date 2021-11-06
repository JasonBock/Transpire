using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Globalization;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	using Verify = AnalyzerVerifier<FindNewGuidViaConstructorAnalyzer>;

	public static class FindNewGuidViaConstructorAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindNewGuidViaConstructorAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindNewGuidViaConstructorDescriptor.Id), 
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Title), 
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNewGuidViaConstructorDescriptor.Message), 
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage), 
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), 
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindNewGuidViaConstructorDescriptor.Id, FindNewGuidViaConstructorDescriptor.Title)), 
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
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenNoGuidIsMadeAsync()
		{
			var code =
@"public static class Test
{
	public static string Make() => new string('a', 1);
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaGuidNewGuidAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static Guid Make() => Guid.NewGuid();
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaStringAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static Guid Make() => new Guid(""83d926c8-9fe6-4cd2-8495-e294e8ade4cb"");
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaNoArgumentConstructorAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static Guid Make() => [|new Guid()|];
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenGuidIsMadeViaTargetTypeNewAsync()
		{
			var code =
@"using System;

public static class Test
{
	public static Guid Make() => [|new()|];
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}
	}
}