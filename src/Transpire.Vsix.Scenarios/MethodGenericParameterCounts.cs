namespace Transpire.Vsix.Scenarios;

public static class MethodGenericParameterCounts
{
	// This should have an informational diagnostic as it has 5 parameters.
	public static void InformationalCase<T0, T1, T2, T3, T4>() { }

	// This should have a warning diagnostic as it has 17 parameters.
	public static void WarningCase<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>() { }

	// I'm not defining a method with 8913 parameters to trip the error case :)
}