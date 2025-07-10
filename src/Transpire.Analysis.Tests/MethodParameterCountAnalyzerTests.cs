using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

internal static class MethodParameterCountAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			0, null, DiagnosticSeverity.Hidden);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			6, DescriptorIdentifiers.MethodParameterCountInfoId, DiagnosticSeverity.Info);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			18, DescriptorIdentifiers.MethodParameterCountWarningId, DiagnosticSeverity.Warning);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			100, DescriptorIdentifiers.MethodParameterCountErrorId, DiagnosticSeverity.Error);

	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			1, null, DiagnosticSeverity.Hidden);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			6, DescriptorIdentifiers.MethodGenericParameterCountInfoId, DiagnosticSeverity.Info);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			18, DescriptorIdentifiers.MethodGenericParameterCountWarningId, DiagnosticSeverity.Warning);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			100, DescriptorIdentifiers.MethodGenericParameterCountErrorId, DiagnosticSeverity.Error);

	private static async Task AnalyzeWithSpecifiedParameterCountAsync(
		int parameterCount, string? diagnosticId, DiagnosticSeverity diagnosticSeverity)
	{
		var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(_ => $"int a{_}"));
		var methodName = parameterCount <= 4 ? "MyMethod" : "MyMethod";

		var code =
			$$"""
			using System;

			public sealed class StringTest
			{
				public void {{methodName}}({{parameters}}) { }
			}
			""";

		if (diagnosticId is not null)
		{
			var diagnostic = new DiagnosticResult(diagnosticId, diagnosticSeverity)
				.WithSpan(5, 14, 5, 22);
			await TestAssistants.RunAnalyzerAsync<MethodParameterCountAnalyzer>(code, [diagnostic]);
		}
		else
		{
			await TestAssistants.RunAnalyzerAsync<MethodParameterCountAnalyzer>(code, []);
		}
	}

	private static async Task AnalyzeWithSpecifiedGenericParameterCountAsync(
		int parameterCount, string? diagnosticId, DiagnosticSeverity diagnosticSeverity)
	{
		var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(_ => $"T{_}"));
		var methodName = parameterCount <= 4 ? "MyMethod" : "MyMethod";

		var code =
			$$"""
			using System;

			public sealed class StringTest
			{
				public void {{methodName}}<{{parameters}}>() { }
			}
			""";

		if (diagnosticId is not null)
		{
			var diagnostic = new DiagnosticResult(diagnosticId, diagnosticSeverity)
				.WithSpan(5, 14, 5, 22);
			await TestAssistants.RunAnalyzerAsync<MethodParameterCountAnalyzer>(code, [diagnostic]);
		}
		else
		{
			await TestAssistants.RunAnalyzerAsync<MethodParameterCountAnalyzer>(code, []);
		}
	}
}