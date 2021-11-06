using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using System.Threading.Tasks;
using Transpire.Descriptors;

namespace Transpire.Tests
{
	internal static class VerifyDispatchProxyGenericParametersAnalyzerTests
	{
		[Test]
		public static async Task AnalyzeWhenPassingClassToCreateAsync()
		{
			var code =
@"using System;
using System.Reflection;

public sealed class DispatchProxyTest
{
	public void MyMethod()
	{
		[|DispatchProxy.Create<Target, TargetProxy>()|];
	}
}

public class Target { }

public class TargetProxy
	: DispatchProxy
{
	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, VerifyDispatchProxyTIsInterfaceDescriptor.Create()).RunAsync().ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenPassingProxyTypeIsAbstractAsync()
		{
			var code =
@"using System;
using System.Reflection;

public sealed class DispatchProxyTest
{
	public void MyMethod()
	{
		[|DispatchProxy.Create<ITarget, TargetProxy>()|];
	}
}

public interface ITarget { }

public abstract class TargetProxy
	: DispatchProxy
{
	public TargetProxy() { }

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Create()).RunAsync().ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenPassingProxyTypeIsSealedAsync()
		{
			var code =
@"using System;
using System.Reflection;

public sealed class DispatchProxyTest
{
	public void MyMethod()
	{
		[|DispatchProxy.Create<ITarget, TargetProxy>()|];
	}
}

public interface ITarget { }

public sealed class TargetProxy
	: DispatchProxy
{
	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, VerifyDispatchProxyTProxyIsNotSealedDescriptor.Create()).RunAsync().ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenPassingProxyTypeWhenParameterlessConstructorIsPrivateAsync()
		{
			var code =
@"using System;
using System.Reflection;

public sealed class DispatchProxyTest
{
	public void MyMethod()
	{
		[|DispatchProxy.Create<ITarget, TargetProxy>()|];
	}
}

public interface ITarget { }

public class TargetProxy
	: DispatchProxy
{
	private TargetProxy() { }

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Create()).RunAsync().ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenPassingProxyTypeWhenPublicConstructorHasParametersAsync()
		{
			var code =
@"using System;
using System.Reflection;

public sealed class DispatchProxyTest
{
	public void MyMethod()
	{
		[|DispatchProxy.Create<ITarget, TargetProxy>()|];
	}
}

public interface ITarget { }

public class TargetProxy
	: DispatchProxy
{
	public TargetProxy(int a) { }

	protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Create()).RunAsync().ConfigureAwait(false);
		}

		[Test]
		public static async Task AnalyzeWhenUsingCreateCorrectlyAsync()
		{
			var code =
@"using System;
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
}";
			await new VerifyDispatchProxyGenericParametersAnalyzerTest(
				code, null).RunAsync().ConfigureAwait(false);
		}

		private sealed class VerifyDispatchProxyGenericParametersAnalyzerTest
			: CSharpAnalyzerTest<VerifyDispatchProxyGenericParametersAnalyzer, NUnitVerifier>
		{
			private readonly DiagnosticDescriptor? descriptor;

			public VerifyDispatchProxyGenericParametersAnalyzerTest(
				string code, DiagnosticDescriptor? descriptor)
				: base()
			{
				this.descriptor = descriptor;
				this.TestState.Sources.Add(code);
			}

			protected override DiagnosticDescriptor? GetDefaultDiagnostic(DiagnosticAnalyzer[] analyzers) =>
				this.descriptor;
		}
	}
}