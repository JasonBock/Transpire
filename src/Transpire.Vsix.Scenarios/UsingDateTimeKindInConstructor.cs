namespace Transpire.Vsix.Scenarios;

public static class UsingDateTimeKindInConstructor
{
	public static DateTime UseUtc() =>
		new(100, DateTimeKind.Utc);

	// This should be flagged as an error.
	public static DateTime UseLocal() =>
		new DateTime(100, DateTimeKind.Local);

	// This should be flagged as an error.
	public static DateTime UseLocalViaTargetTypeNew() =>
		new(100, DateTimeKind.Local);
}