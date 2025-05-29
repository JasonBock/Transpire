using Microsoft.CodeAnalysis.Diagnostics;

namespace Transpire.Analysis;

internal sealed class MethodParameterCountAnalyzerConfiguration
{
	internal const uint DefaultErrorLimit = 32;
	internal const uint DefaultWarningLimit = 16;
	internal const uint DefaultInfoLimit = 4;
	internal const uint Maximum = 8912;

	internal MethodParameterCountAnalyzerConfiguration(AnalyzerConfigOptions options)
	{
		static uint GetValue(AnalyzerConfigOptions options, string id, uint defaultValue) =>
			options.TryGetValue($"dotnet_diagnostic.{id}.limit", out var configurationValue) ?
				(uint.TryParse(configurationValue, out var value) ? value : defaultValue) :
				defaultValue;

		var errorLimit = GetValue(options, DescriptorIdentifiers.MethodParameterCountErrorId,
			MethodParameterCountAnalyzerConfiguration.DefaultErrorLimit);
		var warningLimit = GetValue(options, DescriptorIdentifiers.MethodParameterCountWarningId,
			MethodParameterCountAnalyzerConfiguration.DefaultWarningLimit);
		var infoLimit = GetValue(options, DescriptorIdentifiers.MethodParameterCountInfoId,
			MethodParameterCountAnalyzerConfiguration.DefaultInfoLimit);

		if (errorLimit > MethodParameterCountAnalyzerConfiguration.Maximum ||
			warningLimit >= errorLimit ||
			infoLimit >= warningLimit ||
			errorLimit == 0 ||
			warningLimit == 0 ||
			infoLimit == 0)
		{
			(this.InfoLimit, this.WarningLimit, this.ErrorLimit) =
				(MethodParameterCountAnalyzerConfiguration.DefaultInfoLimit,
				MethodParameterCountAnalyzerConfiguration.DefaultWarningLimit,
				MethodParameterCountAnalyzerConfiguration.DefaultErrorLimit);
		}
		else
		{
			(this.InfoLimit, this.WarningLimit, this.ErrorLimit) =
				(infoLimit, warningLimit, errorLimit);
		}
	}

	internal uint? InfoLimit { get; }
	internal uint? ErrorLimit { get; }
	internal uint? WarningLimit { get; }
}