using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = VerifyAnalyzerWithMultipleDescriptorsTest<MethodParameterCountAnalyzer>;

public static class MethodParameterCountAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableParameterCountAsync() => 
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(0, null).ConfigureAwait(false);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			6, MethodParameterCountInfoDescriptor.Create(0)).ConfigureAwait(false);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			18, MethodParameterCountWarningDescriptor.Create(0)).ConfigureAwait(false);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			100, MethodParameterCountErrorDescriptor.Create(0)).ConfigureAwait(false);

	private static async Task AnalyzeWithSpecifiedParameterCountAsync(int parameterCount, DiagnosticDescriptor? descriptor)
	{
		var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(_ => $"int a{_}"));
		var methodName = parameterCount <= 4 ? "MyMethod" : "[|MyMethod|]";

		var code =
$@"using System;

public sealed class StringTest
{{
	public void {methodName}({parameters}) {{ }}
}}";
		await new Verify(
			code, descriptor).RunAsync().ConfigureAwait(false);
	}
}