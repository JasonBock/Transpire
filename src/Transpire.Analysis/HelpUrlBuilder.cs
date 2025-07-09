namespace Transpire.Analysis;

internal static class HelpUrlBuilder
{
	internal static string Build(string identifier) =>
		$"https://github.com/JasonBock/Transpire/tree/main/docs/{identifier}.md";
}