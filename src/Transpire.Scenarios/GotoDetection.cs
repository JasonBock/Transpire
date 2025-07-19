namespace Transpire.Scenarios;

public static class GotoDetection
{
	public static void UseGoto()
	{
		goto Done;

	Done:
		Console.WriteLine("Done");
	}
}