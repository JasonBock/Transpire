namespace Transpire.Vsix.Scenarios;

public static class MethodParameterCounts
{
	// This should have an informational diagnostic as it has 5 parameters.
	public static void InformationalCase(int a0, int a1, int a2, int a3, int a4) { }

	// This should have a warning diagnostic as it has 17 parameters.
	public static void WarningCase(int a0, int a1, int a2, int a3,
		int a4, int a5, int a6, int a7,
		int a8, int a9, int a10, int a11,
		int a12, int a13, int a14, int a15, int a16) { }

	// I'm not defining a method with 8913 parameters to trip the error case :)
}