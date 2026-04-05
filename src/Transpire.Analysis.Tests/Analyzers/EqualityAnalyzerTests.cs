using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Transpire.Analysis.Analyzers;

namespace Transpire.Analysis.Tests.Analyzers;

internal static class EqualityAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenDiagnosticsAreNotCreatedAsync()
	{
		var code =
			"""
			using Transpire;
			using System;

			#nullable enable
			
			[Equality]
			public partial record Customer(
				Guid Id, [property: Excluded] string Name, uint Age);
			""";

		await TestAssistants.RunAnalyzerAsync<EqualityAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenEqualityExistsOnNonRecordAsync()
	{
		var code =
			"""
			using Transpire;
			using System;

			#nullable enable
			
			[Equality]
			public partial class Customer
			{
				public Guid Id { get; init; }
				[property: Excluded] public string? Name { get; init; }
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.CanOnlyUseEqualityAttributeOnRecordsId, DiagnosticSeverity.Error)
			.WithSpan(6, 2, 6, 10);
		await TestAssistants.RunAnalyzerAsync<EqualityAnalyzer>(code, [diagnostic]);
	}
}