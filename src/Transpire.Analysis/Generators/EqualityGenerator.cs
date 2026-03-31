using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Transpire.Analysis.Generators.Builders;
using Transpire.Analysis.Generators.Models;
using Transpire.Analysis.Models;

namespace Transpire.Analysis;

[Generator]
internal sealed class EqualityGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var records = context.SyntaxProvider
			.ForAttributeWithMetadataName(
				"Transpire.EqualityAttribute",
				(_, _) => true,
				(context, token) =>
				{
					var models = new List<RecordModel>(context.Attributes.Length);

					foreach (var attribute in context.Attributes)
					{
						var recordSymbol = (ITypeSymbol)context.TargetSymbol;

						var modelInformation = RecordModelGenerator.Create(
							recordSymbol, context.SemanticModel.Compilation);

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
			(context, source) => EqualityGenerator.CreateOutput(source, context));
	}

	private static void CreateOutput(ImmutableArray<RecordModel> source, SourceProductionContext context)
	{
		foreach (var record in source)
		{
			var builder = new EqualityBuilder(record);
			context.AddSource(builder.FileName, builder.Text);
		}
	}
}