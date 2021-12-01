namespace Transpire.Vsix.Scenarios;

public static class UnnecessaryInterpolatedStrings
{
	public static void DeclareWithNecessaryInterpolatedString(int value)
	{
		var x = $"This is {value}";
	}

	public static void DeclareWithUnnecessaryInterpolatedString()
	{
		var x = $"This is unnecessary";
	}

	public static void DeclareWithUnnecessaryInterpolatedVerbatimString()
	{
		var x =
 @$"This is 
unnecessary";
	}
}