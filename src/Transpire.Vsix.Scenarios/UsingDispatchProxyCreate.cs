using System;
using System.Reflection;

namespace Transpire.Vsix.Scenarios
{
	public static class UsingDispatchProxyCreate
	{
		public static IUsingDispatchProxy CorrectUsage() => 
			DispatchProxy.Create<IUsingDispatchProxy, CorrectDispatchProxy>();

		// This should fail because T is getting a class
		public static UsingDispatchProxy PassingClassToT() =>
			DispatchProxy.Create<UsingDispatchProxy, CorrectDispatchProxy>();

		// This should fail because TProxy is getting an abstract type
		public static IUsingDispatchProxy PassingAbstractProxyType() =>
			DispatchProxy.Create<IUsingDispatchProxy, AbstractDispatchProxy>();

		// This should fail because TProxy is getting a sealed type
		public static IUsingDispatchProxy PassingSealedProxyType() =>
			DispatchProxy.Create<IUsingDispatchProxy, SealedDispatchProxy>();

		// This should fail because TProxy is getting a type with a private constructor
		public static IUsingDispatchProxy PassingProxyTypeWithPrivateConstructor() =>
			DispatchProxy.Create<IUsingDispatchProxy, PrivateConstructorDispatchProxy>();

		// This should fail because TProxy is getting a type with a constructor with parameters
		public static IUsingDispatchProxy PassingProxyTypeWithConstructorHavingParameters() =>
			DispatchProxy.Create<IUsingDispatchProxy, ConstructorWithParametersDispatchProxy>();
	}

	public interface IUsingDispatchProxy { }

	public class UsingDispatchProxy { }

	public abstract class AbstractDispatchProxy
		: DispatchProxy
	{
		public AbstractDispatchProxy() { }

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
	}

	public sealed class SealedDispatchProxy
		: DispatchProxy
	{
		public SealedDispatchProxy() { }

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
	}

	public class PrivateConstructorDispatchProxy
		: DispatchProxy
	{
		private PrivateConstructorDispatchProxy() { }

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
	}

	public class ConstructorWithParametersDispatchProxy
		: DispatchProxy
	{
		public ConstructorWithParametersDispatchProxy(int a) { }

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
	}

	public class CorrectDispatchProxy
		: DispatchProxy
	{
		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => throw new NotImplementedException();
	}
}