using NUnit.Framework;
using Transpire.Analysis;

namespace Transpire.Completions.Tests;

internal static class DeregionizerCodeFixTests
{
	[Test]
	public static void VerifyGetFixableDiagnosticIds()
	{
		var fix = new DeregionizeCodeFix();
		var ids = fix.FixableDiagnosticIds;

		Assert.Multiple(() =>
		{
			Assert.That(ids, Has.Length.EqualTo(1), nameof(ids.Length));
			Assert.That(ids[0], Is.EqualTo(DescriptorIdentifiers.DeregionizeId), nameof(DescriptorIdentifiers.DeregionizeId));
		});
	}

	[Test]
	public static async Task VerifyGetFixesWhenRegionAndEndRegionExistAsync()
	{
		var originalCode =
			"""
			[|using System;
			
			#region Useless code
			public interface IAmUseless { }
			#endregion
			
			public interface IAmUseful
			{
				void DoWork();
			}|]
			""";
		var fixedCode =
			"""
			using System;
			
			public interface IAmUseless { }

			public interface IAmUseful
			{
				void DoWork();
			}
			""";

		await TestAssistants.RunCodeFixAsync<DeregionizeAnalyzer, DeregionizeCodeFix>(
			originalCode, fixedCode, 0);
	}
}