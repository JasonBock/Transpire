using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using NuGet.Frameworks;

namespace Transpire.Completions.Tests;

internal static class TestAssistants
{
	internal static async Task RunCodeFixAsync<TAnalyzer, TCodeFix>(string originalCode, string fixedCode, int codeActionIndex)
		where TAnalyzer : DiagnosticAnalyzer, new()
		where TCodeFix : CodeFixProvider, new()
	{
		var test = new CodeFixTest<TAnalyzer, TCodeFix>
		{
			ReferenceAssemblies = TestAssistants.GetNet90(),
			TestCode = originalCode,
			FixedCode = fixedCode,
			CodeActionIndex = codeActionIndex,
		};

		test.TestState.AdditionalReferences.Add(typeof(TAnalyzer).Assembly);
		test.TestState.AdditionalReferences.Add(typeof(TCodeFix).Assembly);

		await test.RunAsync();
	}

	private static ReferenceAssemblies GetNet90()
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net9.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net9.0' target framework
			throw new NotSupportedException("The 'net9.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net9.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "9.0.5"),
			 Path.Combine("ref", "net9.0"));
	}
}