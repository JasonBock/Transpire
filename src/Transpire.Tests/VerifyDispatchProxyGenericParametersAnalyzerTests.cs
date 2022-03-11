using NUnit.Framework;
using Transpire.Descriptors;

namespace Transpire.Tests;

using Verify = VerifyAnalyzerWithMultipleDescriptorsTest<VerifyDispatchProxyGenericParametersAnalyzer>;

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
		await new Verify(code, VerifyDispatchProxyTIsInterfaceDescriptor.Create())
			.RunAsync().ConfigureAwait(false);
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
		await new Verify(code, VerifyDispatchProxyTProxyIsNotAbstractDescriptor.Create())
			.RunAsync().ConfigureAwait(false);
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
		await new Verify(code, VerifyDispatchProxyTProxyIsNotSealedDescriptor.Create())
			.RunAsync().ConfigureAwait(false);
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
		await new Verify(code, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Create())
			.RunAsync().ConfigureAwait(false);
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
		await new Verify(code, VerifyDispatchProxyTProxyHasPublicParameterlessConstructorDescriptor.Create())
			.RunAsync().ConfigureAwait(false);
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
		await new Verify(code, null)
			.RunAsync().ConfigureAwait(false);
	}
}