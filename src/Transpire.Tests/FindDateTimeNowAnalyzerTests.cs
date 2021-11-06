using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing.NUnit;
using NUnit.Framework;
using System.Globalization;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	using Verify = AnalyzerVerifier<FindDateTimeNowAnalyzer>;

	public static class FindDateTimeNowAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindDateTimeNowAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindDateTimeNowDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeNowDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindDateTimeNowDescriptor.Message),
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindDateTimeNowDescriptor.Id, FindDateTimeNowDescriptor.Title)),
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeNowAsync()
		{
			var code =
@"using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = [|DateTime.Now|];
	}
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeNowWithAliasAsync()
		{
			var code =
@"using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = [|DT.Now|];
	}
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeUtcNowAsync()
		{
			var code =
@"using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DateTime.UtcNow;
	}
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeUtcNowWithAliasAsync()
		{
			var code =
@"using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DT.UtcNow;
	}
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenCallingNowAsAPropertyAsync()
		{
			var code =
@"using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = this.Now;
	}

	public string Now { get; set; }
}";
			await Verify.VerifyAnalyzerAsync(code).ConfigureAwait(false);
		}
	}
}