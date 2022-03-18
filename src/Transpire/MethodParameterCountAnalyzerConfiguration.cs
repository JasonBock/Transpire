using Microsoft.CodeAnalysis.Diagnostics;
using Transpire.Descriptors;

namespace Transpire;

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

		var errorLimit = GetValue(options, MethodParameterCountErrorDescriptor.Id,
			MethodParameterCountAnalyzerConfiguration.DefaultErrorLimit);
		var warningLimit = GetValue(options, MethodParameterCountWarningDescriptor.Id,
			MethodParameterCountAnalyzerConfiguration.DefaultWarningLimit);
		var infoLimit = GetValue(options, MethodParameterCountInfoDescriptor.Id,
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