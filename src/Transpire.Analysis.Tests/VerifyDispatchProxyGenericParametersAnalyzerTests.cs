using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Transpire.Analysis.Tests;

internal static class VerifyDispatchProxyGenericParametersAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenPassingClassToCreateAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<Target, TargetProxy>();
				}
			}

			public class Target { }

			public class TargetProxy
				: DispatchProxy
			{
				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => 
					throw new NotImplementedException();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.VerifyDispatchProxyTIsInterfaceId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 46);
		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenPassingProxyTypeIsAbstractAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<ITarget, TargetProxy>();
				}
			}

			public interface ITarget { }

			public abstract class TargetProxy
				: DispatchProxy
			{
				public TargetProxy() { }

				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => 
					throw new NotImplementedException();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotAbstractId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 47);
		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenPassingProxyTypeIsSealedAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<ITarget, TargetProxy>();
				}
			}

			public interface ITarget { }

			public sealed class TargetProxy
				: DispatchProxy
			{
				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.VerifyDispatchProxyTProxyIsNotSealedId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 47);
		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenPassingProxyTypeWhenParameterlessConstructorIsPrivateAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<ITarget, TargetProxy>();
				}
			}

			public interface ITarget { }

			public class TargetProxy
				: DispatchProxy
			{
				private TargetProxy() { }

				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.VerifyDispatchProxyTProxyHasPublicParameterlessConstructorId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 47);
		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenPassingProxyTypeWhenPublicConstructorHasParametersAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<ITarget, TargetProxy>();
				}
			}

			public interface ITarget { }

			public class TargetProxy
				: DispatchProxy
			{
				public TargetProxy(int a) { }

				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
			}
			""";

		var diagnostic = new DiagnosticResult(DescriptorIdentifiers.VerifyDispatchProxyTProxyHasPublicParameterlessConstructorId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 47);
		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenUsingCreateCorrectlyAsync()
	{
		var code =
			"""
			using System;
			using System.Reflection;

			public sealed class DispatchProxyTest
			{
				public void MyMethod()
				{
					DispatchProxy.Create<ITarget, TargetProxy>();
				}
			}

			public interface ITarget { }

			public class TargetProxy
				: DispatchProxy
			{
				protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
			}
			""";

		await TestAssistants.RunAnalyzerAsync<VerifyDispatchProxyGenericParametersAnalyzer>(code, []);
	}
}