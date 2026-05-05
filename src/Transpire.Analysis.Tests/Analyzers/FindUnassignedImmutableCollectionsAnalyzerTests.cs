using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Collections.Immutable;
using System.Globalization;
using Transpire.Analysis.Analyzers;
using Transpire.Analysis.Descriptors;

namespace Transpire.Analysis.Tests.Analyzers;

internal static class FindUnassignedImmutableCollectionsAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new FindUnassignedImmutableCollectionsAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(diagnostics, Has.Length.EqualTo(1), nameof(diagnostics.Length));

			var diagnostic = diagnostics[0];

			Assert.That(diagnostic.Id, Is.EqualTo(DescriptorIdentifiers.FindUnassignedImmutableCollectionsId),
				nameof(DiagnosticDescriptor.Id));
			Assert.That(diagnostic.Title.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindUnassignedImmutableCollectionsDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(diagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture), Is.EqualTo(FindUnassignedImmutableCollectionsDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(diagnostic.Category, Is.EqualTo(DescriptorConstants.Usage),
				nameof(DiagnosticDescriptor.Category));
			Assert.That(diagnostic.DefaultSeverity, Is.EqualTo(DiagnosticSeverity.Error),
				nameof(DiagnosticDescriptor.DefaultSeverity));
			Assert.That(diagnostic.IsEnabledByDefault, Is.True,
				nameof(DiagnosticDescriptor.IsEnabledByDefault));
			Assert.That(diagnostic.HelpLinkUri,
				Is.EqualTo(
					HelpUrlBuilder.Build(
						DescriptorIdentifiers.FindUnassignedImmutableCollectionsId)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		}
	}

	[Test]
	public static async Task AnalyzeWhenNoInvocationExistsAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() { }
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenInvocationIsNotOnImmutableCollectionAsync()
	{
		var code =
			"""
			public static class Test
			{
				public static void Run() 
				{ 
					var data = "test";
					var subData = data.Substring(1);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnArrayWhenInvocationReturnsVoidAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableArray<int> items = [2, 3, 4];
					var destination = new int[3];
					items.CopyTo(destination);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnArrayWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableArray<int> items = [2, 3, 4];
					items.IndexOf(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnArrayWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableArray<int> items = [2, 3, 4];
					items = items.Add(20);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnArrayWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableArray<int> items = [2, 3, 4];
					items.Add(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 16);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnDictionaryWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableDictionary<int, int> items = [];
					items.ContainsKey(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnDictionaryWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableDictionary<int, int> items = [];
					items = items.Add(3, 4);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnDictionaryWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableDictionary<int, int> items = [];
					items.Add(3, 4);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 18);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnHashSetWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableHashSet<int> items = [2, 3, 4];
					items.Contains(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnHashSetWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableHashSet<int> items = [2, 3, 4];
					items = items.Add(20);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnHashSetWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableHashSet<int> items = [2, 3, 4];
					items.Add(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 16);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnListWhenInvocationReturnsVoidAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableList<int> items = [2, 3, 4];
					var destination = new int[3];
					items.CopyTo(destination);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnListWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableList<int> items = [2, 3, 4];
					items.IndexOf(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnListWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableList<int> items = [2, 3, 4];
					items = items.Add(20);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnListWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableList<int> items = [2, 3, 4];
					items.Add(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 16);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnQueueWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableQueue<int> items = [2, 3, 4];
					items.Equals(null);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnQueueWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableQueue<int> items = [2, 3, 4];
					items = items.Enqueue(20);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnQueueWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableQueue<int> items = [2, 3, 4];
					items.Enqueue(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 20);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnSortedSetWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableSortedSet<int> items = [2, 3, 4];
					items.Contains(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnSortedSetWhenInvocationCapturesSameTypeAsync()
	{
		ImmutableSortedSet<int> items = [2, 3, 4];
		items = items.Add(3);

		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableSortedSet<int> items = [2, 3, 4];
					items = items.Add(3);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnSortedSetWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableSortedSet<int> items = [2, 3, 4];
					items.Add(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 16);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeOnStackWhenInvocationDoesNotReturnSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableStack<int> items = [2, 3, 4];
					items.Peek();
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnStackWhenInvocationCapturesSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableStack<int> items = [2, 3, 4];
					items = items.Push(20);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeOnStackWhenInvocationDoesNotCaptureSameTypeAsync()
	{
		var code =
			"""
			using System.Collections.Immutable;

			public static class Test
			{
				public static void Run() 
				{
					ImmutableStack<int> items = [2, 3, 4];
					items.Push(20);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(
			DescriptorIdentifiers.FindUnassignedImmutableCollectionsId, DiagnosticSeverity.Error)
			.WithSpan(8, 3, 8, 17);
		await TestAssistants.RunAnalyzerAsync<FindUnassignedImmutableCollectionsAnalyzer>(code, [diagnostic]);
	}
}