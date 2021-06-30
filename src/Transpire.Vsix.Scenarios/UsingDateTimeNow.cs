using System;

namespace Transpire.Vsix.Scenarios
{
	public static class UsingDateTimeNow
	{
		// This should be flagged as an error.
		public static DateTime GetViaNow() => DateTime.Now;

		public static DateTime GetViaUtcNow() => DateTime.UtcNow;
	}
}