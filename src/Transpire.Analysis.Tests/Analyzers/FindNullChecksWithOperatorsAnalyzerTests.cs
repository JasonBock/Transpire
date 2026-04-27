using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;
using Transpire.Analysis.Analyzers;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests.Analyzers;

internal static class FindNullChecksWithOperatorsAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindNullChecksWithOperatorsAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.FindNullChecksWithOperatorsId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNullChecksWithOperatorsDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindNullChecksWithOperatorsDescriptor.Message),
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
						DescriptorIdentifiers.FindNullChecksWithOperatorsId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		}
	}

	[Test]
	public static async Task AnalyzeWhenBinaryExpressionDoesNotExistAsync()
	{
		var code =
			"""
			using System;
			
			public static class Test
			{
				public static void Run() 
				{ 
					Console.WriteLine("Continue");
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindNullChecksWithOperatorsAnalyzer>(code, []);
	}

	[TestCase(""" value == "3" """)]
	[TestCase(""" value != "3" """)]
	[TestCase(""" "3" == value """)]
	[TestCase(""" "3" != value """)]
	public static async Task AnalyzeWhenBinaryExpressionExistsWithNoNullComparisonAsync(string expression)
	{
		var code =
			$$"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					{{expression}};
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindNullChecksWithOperatorsAnalyzer>(code, []);
	}

	[TestCase(""" value == null """)]
	[TestCase(""" value != null """)]
	[TestCase(""" null == value """)]
	[TestCase(""" null != value """)]
	public static async Task AnalyzeWhenBinaryExpressionExistsWithNullComparisonAsync(string expression)
	{
		var code =
			$$"""
			using System;
			
			public static class Test
			{
				public static bool Run(string value) =>
					{{expression}};
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindNullChecksWithOperatorsId, DiagnosticSeverity.Error)
			.WithSpan(6, 4, 6, 17);
		await TestAssistants.RunAnalyzerAsync<FindNullChecksWithOperatorsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenBinaryExpressionExistsWithinExpressionAsync()
	{
		var code =
			"""
			using System;
			using System.Linq.Expressions;
			
			public static class Test
			{
				public static void Run()
				{
					Expression<Func<string, bool>> expr = value => value == null;
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindNullChecksWithOperatorsAnalyzer>(code, []);
	}
}