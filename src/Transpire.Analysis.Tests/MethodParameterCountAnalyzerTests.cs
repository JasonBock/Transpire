﻿using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests;

using Verify = VerifyAnalyzerWithMultipleDescriptorsTest<MethodParameterCountAnalyzer>;

internal static class MethodParameterCountAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableParameterCountAsync() => 
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(0, null);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			6, MethodParameterCountInfoDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			18, MethodParameterCountWarningDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedParameterCountAsync(
			100, MethodParameterCountErrorDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasAcceptableGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(1, null);

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanInfoLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			6, MethodGenericParameterCountInfoDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanWarningLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			18, MethodGenericParameterCountWarningDescriptor.Create(0));

	[Test]
	public static async Task AnalyzeWhenMethodHasMoreThanErrorLevelGenericParameterCountAsync() =>
		await MethodParameterCountAnalyzerTests.AnalyzeWithSpecifiedGenericParameterCountAsync(
			100, MethodGenericParameterCountErrorDescriptor.Create(0));

	private static async Task AnalyzeWithSpecifiedParameterCountAsync(int parameterCount, DiagnosticDescriptor? descriptor)
	{
		var parameters = string.Join(", ", Enumerable.Range(0, parameterCount).Select(_ => $"int a{_}"));
		var methodName = parameterCount <= 4 ? "MyMethod" : "[|MyMethod|]";

		var code =
			$$"""
			using System;

			public sealed class StringTest
			{
				public void {{methodName}}({{parameters}}) { }
			}
			""";
		await new Verify(
			code, descriptor).RunAsync();
	}

	private static async Task AnalyzeWithSpecifiedGenericParameterCountAsync(int parameterCount, DiagnosticDescriptor? descriptor)
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