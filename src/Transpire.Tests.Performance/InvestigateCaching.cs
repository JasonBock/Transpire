using BenchmarkDotNet.Attributes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Transpire.Tests.Performance
{
	[MemoryDiagnoser]
	public class InvestigateCaching
	{
		private CSharpCompilation? compilation;

		[ParamsSource(nameof(GetCode))]
		public string? Code { get; set; }

		public IEnumerable<string> GetCode()
		{
			yield return "var id1 = new System.Guid();";
			yield return "var id1 = new System.Guid(); var id2 = new System.Guid(); var id3 = new System.Guid(); var id4 = new System.Guid();";
			yield return
@"using System;

public sealed class Customer
{
	public Customer(Guid id, string name) =>
		(this.Id, this.Name) = (id, name);

	public string Name { get; }
	public Guid Id { get; }
}

public static class CustomerFactor
{
	public static Customer Create(string name) =>
		new Customer(new(), name);
}";
		}

		[GlobalSetup]
		public void GlobalSetup() => this.compilation = InvestigateCaching.GetCompilation(this.Code!);

		[Benchmark(Baseline = true)]
		public async Task<int> GetDiagnosticCountAsync() => 
			(await this.GetDiagnosticsAsync(new FindNewGuidViaConstructorAnalyzer())).Length;

		[Benchmark]
		public async Task<int> GetDiagnosticCountWithCachingAsync() =>
			(await this.GetDiagnosticsAsync(new FindNewGuidViaConstructorWithCachingAnalyzer())).Length;

		internal async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(DiagnosticAnalyzer analyzer)
		{
			var compilation = this.compilation!.WithAnalyzers(ImmutableArray.Create(analyzer));
			return await compilation.GetAnalyzerDiagnosticsAsync();
		}

		internal static CSharpCompilation GetCompilation(string source)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(source);
			var references = AppDomain.CurrentDomain.GetAssemblies()
				.Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
				.Select(_ => MetadataReference.CreateFromFile(_.Location));
			return CSharpCompilation.Create("analyzer", new SyntaxTree[] { syntaxTree },
				references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
		}
	}
}