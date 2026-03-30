namespace Transpire;

/// <summary>
/// Specifies that a property should be excluded from equality operations on a record.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class OrderedAttribute
	: Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="OrderedAttribute"/> class with the specified order value.
	/// </summary>
	/// <param name="order">The order in which this attribute should be considered when determining equality.</param>
	public OrderedAttribute(uint order) =>
		this.Order = order;

	/// <summary>
	/// Gets the order in which this item appears relative to others, if specified.
	/// </summary>
	public uint? Order { get; }
}