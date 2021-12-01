namespace Transpire.Vsix.Scenarios;

public static class UsingDateTimeNow
{
	// This should be flagged as an error.
	public static DateTime GetViaNow() => DateTime.Now;

	// This should be flagged as an error.
	public static DateTime GetViaNew() => new DateTime();

	// This should be flagged as an error.
	public static DateTime GetViaNewTargetTypeNew() => new();

	public static DateTime GetViaUtcNow() => DateTime.UtcNow;
}