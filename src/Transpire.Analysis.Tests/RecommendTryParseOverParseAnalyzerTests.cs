using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

internal static class RecommendTryParseOverParseAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new RecommendTryParseOverParseAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		Assert.Multiple(() =>
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.RecommendTryParseOverParseId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendTryParseOverParseDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(RecommendTryParseOverParseDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri, 
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.RecommendTryParseOverParseId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		});
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseAsync()
	{
		var code =
			"""
			using System;

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = int.Parse("3");
				}
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.RecommendTryParseOverParseId, DiagnosticSeverity.Error)
			.WithSpan(7, 11, 7, 25);
		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithNoCorrespondingTryParseAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public static MyParser Parse(string value) => new();
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithTryParseAndIncorrectParameterCountAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public static MyParser Parse(string value) => new();
				public static bool TryParse(string value) => true;
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithTryParseAsInstanceAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public static MyParser Parse(string value) => new();
				
				public bool TryParse(string value, out MyParser result) 
				{
					result = new();
					return true;
				}
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithTryParseThatHasNoOutParameterAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public static MyParser Parse(string value) => new();
				public static bool TryParse(string value, MyParser result) => true;
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithTryParseThatHasIncorrectReturnTypeAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public static MyParser Parse(string value) => new();

				public static int TryParse(string value, out MyParser result)
				{
					result = new();
					return 0;
				}
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithIncorrectNumberOfParametersAsync()
	{
		var code =
			"""
			using System;

			internal static class MyParser
			{
				public static int Parse(string value, bool isSomething) => 0;
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3", true);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithIncorrectParameterTypeAsync()
	{
		var code =
			"""
			using System;

			internal static class MyParser
			{
				public static int Parse(bool isSomething) => 0;
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse(true);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseWithReturnTypeAndContainingTypeDoNotMatchAsync()
	{
		var code =
			"""
			using System;

			internal static class MyParser
			{
				public static int Parse(string value) => 0;
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = MyParser.Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenCallingParseAsInstanceMethodAsync()
	{
		var code =
			"""
			using System;

			public class MyParser
			{
				public MyParser Parse(string value) => new MyParser();
			}

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					var x = new MyParser().Parse("3");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenNotCallingParseAsync()
	{
		var code =
			"""
			using System;

			public sealed class IntParseTest
			{
				public void MyMethod()
				{
					int.TryParse("3", out var x);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<RecommendTryParseOverParseAnalyzer>(code, []);
	}
}