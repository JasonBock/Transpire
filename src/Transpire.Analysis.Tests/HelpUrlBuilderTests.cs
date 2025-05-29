using NUnit.Framework;

namespace Transpire.Analysis.Tests;

internal static class HelpUrlBuilderTests
{
	[Test]
	public static void Build() =>
		Assert.That(HelpUrlBuilder.Build("a", "b"),
			Is.EqualTo("https://github.com/JasonBock/Transpire/tree/main/docs/a-b.md"));
}