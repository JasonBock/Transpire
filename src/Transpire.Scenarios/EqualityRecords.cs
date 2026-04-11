namespace Transpire.Scenarios;

[Equality]
public partial record Customer(
	Guid Id, [property: Excluded] string Name, uint Age);

[Equality]
public partial record AllPropertiesExcludedCustomer(
	[property: Excluded] Guid Id, [property: Excluded] string Name, [property: Excluded] uint Age);

[Equality]
public partial record ExcludedAndOrderedUsedTogetherCustomer(
	Guid Id, [property: Excluded, Ordered(3u)] string Name, uint Age);

[Equality]
public partial class NonRecordCustomer
{
	public Guid Id { get; init; }
	[property: Excluded] public string? Name { get; init; }
}

[Equality]
public partial record ExcludedAndOrderedNotUsedCustomer(
	Guid Id, string Name, uint Age);

[Equality]
public partial record OnlyOnePropertyOrderedCustomer([property: Ordered(3u)] Guid Id);

public partial record OrderedExistsAndEqualityDoesNotCustomer(
	Guid Id, [property: Ordered(3u)] string Name, uint Age);

public partial record ExcludedExistsAndEqualityDoesNotCustomer(
	Guid Id, [property: Excluded] string Name, uint Age);