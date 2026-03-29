namespace Transpire;

/// <summary>
/// Specifies how a property participates in value-based equality comparisons for a record.
/// </summary>
/// <remarks>
/// Apply this attribute to a property defined on a record to control its inclusion and ordering in equality operations.
/// The attribute can indicate whether the property should be
/// considered for equality and, if so, its relative order in the comparison sequence. This is useful when customizing
/// equality semantics beyond the default behavior.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class EqualityAttribute
  : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EqualityAttribute"/> class with the specified order value.
	/// </summary>
	/// <param name="order">The order in which this attribute should be considered when determining equality.</param>
	public EqualityAttribute(uint order) => 
		this.Order = order;

	/// <summary>
	/// Initializes a new instance of the <see cref="EqualityAttribute"/> class with the specified record usage option.
	/// </summary>
	/// <param name="usage">A value that specifies how the attribute should be applied to record usage scenarios.</param>
	public EqualityAttribute(RecordUsage usage) => 
		this.Usage = usage;

	/// <summary>
	/// Initializes a new instance of the <see cref="EqualityAttribute"/> class with the specified record usage and order.
	/// </summary>
	/// <param name="usage">Specifies how the attribute is used in record comparisons.</param>
	/// <param name="order">The order in which the attribute is considered during equality checks.</param>
	public EqualityAttribute(RecordUsage usage, uint order) =>
	  (this.Usage, this.Order) = (usage, order);

	/// <summary>
	/// Gets the order in which this item appears relative to others, if specified.
	/// </summary>
	public uint? Order { get; }

	/// <summary>
	/// Gets the usage details associated with a property.
	/// </summary>
	public RecordUsage Usage { get; }
}