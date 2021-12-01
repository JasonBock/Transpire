namespace Transpire;

internal static class HelpUrlBuilder
{
	internal static string Build(string identifier, string title) =>
		$"https://github.com/JasonBock/Transpire/tree/main/docs/{identifier}-{title}.md";
}