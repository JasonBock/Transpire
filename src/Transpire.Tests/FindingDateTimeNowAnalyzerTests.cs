using Microsoft.CodeAnalysis;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	public static class FindingDateTimeNowAnalyzerTests
	{
		[Test]
		public static void VerifySupportedDiagnostics()
		{
			var analyzer = new FindingDateTimeNowAnalyzer();
			var diagnostics = analyzer.SupportedDiagnostics;

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));

				var diagnostic = diagnostics[0];

				Assert.That(diagnostic.Id, Is.EqualTo(FindingDateTimeNowDescriptor.Id),
					nameof(DiagnosticDescriptor.Id));
				Assert.That(diagnostic.Title.ToString(), Is.EqualTo(FindingDateTimeNowDescriptor.Title),
					nameof(DiagnosticDescriptor.Title));
				Assert.That(diagnostic.MessageFormat.ToString(), Is.EqualTo(FindingDateTimeNowDescriptor.Message),
					nameof(DiagnosticDescriptor.MessageFormat));
				Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
					nameof(DiagnosticDescriptor.Category));
				Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
					nameof(DiagnosticDescriptor.DefaultSeverity));
				Assert.That(diagnostic.IsEnabledByDefault, Is.True,
					nameof(DiagnosticDescriptor.IsEnabledByDefault));
				Assert.That(diagnostic.HelpLinkUri, Is.EqualTo(HelpUrlBuilder.Build(FindingDateTimeNowDescriptor.Id, FindingDateTimeNowDescriptor.Title)),
					nameof(DiagnosticDescriptor.HelpLinkUri));
			});
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeNow()
		{
			var code =
@"using System;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DateTime.Now;
	}
}";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindingDateTimeNowDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindingDateTimeNowDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeNowWithAlias()
		{
			var code =
@"using DT = System.DateTime;

public sealed class DateTimeTest
{
	public void MyMethod()
	{
		var x = DT.Now;
	}
}";
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(code);

			Assert.Multiple(() =>
			{
				Assert.That(diagnostics.Length, Is.EqualTo(1), nameof(diagnostics.Length));
				var descriptor = diagnostics[0].Descriptor;
				Assert.That(descriptor.Id, Is.EqualTo(FindingDateTimeNowDescriptor.Id), nameof(descriptor.Id));
				Assert.That(descriptor.Title.ToString(), Is.EqualTo(FindingDateTimeNowDescriptor.Title), nameof(descriptor.Title));
				Assert.That(descriptor.Category, Is.EqualTo(DescriptorConstants.Usage), nameof(descriptor.Category));
				Assert.That(descriptor.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error), nameof(descriptor.DefaultSeverity));
			});
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeUtcNow()
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
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(code);
			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenCallingDateTimeUtcNowWithAlias()
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
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(code);
			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}

		[Test]
		public static async Task AnalyzeWhenCallingNowAsAProperty()
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
			var diagnostics = await TestAssistants.GetDiagnosticsAsync<FindingDateTimeNowAnalyzer>(code);
			Assert.That(diagnostics.Length, Is.EqualTo(0), nameof(diagnostics.Length));
		}
	}
}