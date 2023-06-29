using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = VerifyAnalyzerWithMultipleDescriptorsTest<MethodGenericParameterCountAnalyzer>;

public static class MethodGenericParameterCountAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableParameterCountAsync() =>
		await MethodGenericParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(1, null);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelParameterCountAsync() =>
		await MethodGenericParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			6, MethodGenericParameterCountInfoDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelParameterCountAsync() =>
		await MethodGenericParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			18, MethodGenericParameterCountWarningDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelParameterCountAsync() =>
		await MethodGenericParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			100, MethodGenericParameterCountErrorDescriptor.Create(0));

	private static async Task AnalyzeWithSpecifiedParameterCountAsync(int parameterCount, DiagnosticDescriptor? descriptor)
	{
		var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(_ => $"T{_}"));
		var methodName = parameterCount <= 4 ? "MyMethod" : "[|MyMethod|]";

		var code =
			$$"""
			using System;

			public sealed class StringTest
			{
				public void {{methodName}}<{{parameters}}>() { }
			}
			""";
		await new Verify(
			code, descriptor).RunAsync();
	}
}