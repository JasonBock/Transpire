using Microsoft.CodeAnalysis.Diagnostics;
using Transpire.Descriptors;

namespace Transpire;

internal sealed class MethodGenericParameterCountAnalyzerConfiguration
{
	internal const uint DefaultErrorLimit = 32;
	internal const uint DefaultWarningLimit = 16;
	internal const uint DefaultInfoLimit = 4;
	internal const uint Maximum = 32769;

	internal MethodGenericParameterCountAnalyzerConfiguration(AnalyzerConfigOptions options)
	{
		static uint GetValue(AnalyzerConfigOptions options, string id, uint defaultValue) =>
			options.TryGetValue($"dotnet_diagnostic.{id}.limit", out var configurationValue) ?
				(uint.TryParse(configurationValue, out var value) ? value : defaultValue) :
				defaultValue;

		var errorLimit = GetValue(options, MethodGenericParameterCountErrorDescriptor.Id,
			MethodGenericParameterCountAnalyzerConfiguration.DefaultErrorLimit);
		var warningLimit = GetValue(options, MethodGenericParameterCountWarningDescriptor.Id,
			MethodGenericParameterCountAnalyzerConfiguration.DefaultWarningLimit);
		var infoLimit = GetValue(options, MethodGenericParameterCountInfoDescriptor.Id,
			MethodGenericParameterCountAnalyzerConfiguration.DefaultInfoLimit);

		if (errorLimit > MethodGenericParameterCountAnalyzerConfiguration.Maximum ||
			warningLimit >= errorLimit ||
			infoLimit >= warningLimit ||
			errorLimit == 0 ||
			warningLimit == 0 ||
			infoLimit == 0)
		{
			(this.InfoLimit, this.WarningLimit, this.ErrorLimit) =
				(MethodGenericParameterCountAnalyzerConfiguration.DefaultInfoLimit,
				MethodGenericParameterCountAnalyzerConfiguration.DefaultWarningLimit,
				MethodGenericParameterCountAnalyzerConfiguration.DefaultErrorLimit);
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