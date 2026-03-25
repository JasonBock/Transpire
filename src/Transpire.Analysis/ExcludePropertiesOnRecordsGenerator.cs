using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using Transpire.Analysis.Models;

namespace Transpire.Analysis;

[Generator]
internal sealed class ExcludePropertiesOnRecordsGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var records = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Transpire.ExcludeAttribute",
				(_, _) => true,
				(context, token) =>
				{
					var models = new List<RecordModel>(context.Attributes.Length);

					foreach (var attribute in context.Attributes)
					{
						var propertyNames = new HashSet<string>((string[])attribute.ConstructorArguments[1].Value!);
						var recordSymbol = (ITypeSymbol)context.TargetSymbol;

						var modelInformation = RecordModelGenerator.Create(recordSymbol, propertyNames);

						if (modelInformation.Model is not null)
						{
							models.Add(modelInformation.Model);
						}
					}

					return models;
				})
			.SelectMany((models, _) => models);

		var collectedRecords = records.Collect();

		context.RegisterSourceOutput(collectedRecords,
			(context, source) => ExcludePropertiesOnRecordsGenerator.CreateOutput(source, context));
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