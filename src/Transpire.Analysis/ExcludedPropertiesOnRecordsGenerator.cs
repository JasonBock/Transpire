using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using Transpire.Analysis.Models;

namespace Transpire.Analysis;

[Generator]
internal sealed class ExcludedPropertiesOnRecordsGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var records = context.SyntaxProvider
			.ForAttributeWithMetadataName("Transpire.ExcludeAttribute", (_, _) => true,
			(context, token) =>
			{
				var attributeClass = context.Attributes[0];
				var propertyNames = (string[])attributeClass.ConstructorArguments[1].Value!;
				var recordSymbol = (ITypeSymbol)context.TargetSymbol;

				return new RecordModel(
					recordSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), [.. propertyNames]);
			});

		var collectedRecords = records.Collect();

		context.RegisterSourceOutput(collectedRecords,
			(context, source) => ExcludedPropertiesOnRecordsGenerator.CreateOutput(source, context));
	}

   private static void CreateOutput(ImmutableArray<RecordModel> source, SourceProductionContext context)
	{
		foreach (var record in source)
		{
			var code =
			$$"""
			using System;

			public sealed partial record {{record.RecordFullyQualifiedName}}
				: IEquatable<{{record.RecordFullyQualifiedName}}>
			{
				// TODO: Lots of stuff.
			}
			""";

			var text = SourceText.From(
				string.Join(Environment.NewLine, code),
				Encoding.UTF8);

			context.AddSource($"{record.RecordFullyQualifiedName}.g.cs", text);
		}
	}
}