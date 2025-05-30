﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Transpire.Analysis.Tests;

internal sealed class VerifyAnalyzerWithMultipleDescriptorsTest<TAnalyzer>
	: CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
	where TAnalyzer : DiagnosticAnalyzer, new()
{
	private readonly DiagnosticDescriptor? descriptor;

	public VerifyAnalyzerWithMultipleDescriptorsTest(
		string code, DiagnosticDescriptor? descriptor)
		: base()
	{
		this.descriptor = descriptor;
		this.TestState.Sources.Add(code);
	}

	protected override DiagnosticDescriptor? GetDefaultDiagnostic(DiagnosticAnalyzer[] analyzers) =>
		this.descriptor;
}