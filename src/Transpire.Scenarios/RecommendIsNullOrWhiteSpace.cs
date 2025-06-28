namespace Transpire.Scenarios;

public static class RecommendIsNullOrWhiteSpace
{
	public static bool Test(string? content) => string.IsNullOrEmpty(content);
}