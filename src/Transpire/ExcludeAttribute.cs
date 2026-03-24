namespace Transpire;

/// <summary>
/// Allows a user to target properties by name on a record
/// that should be excluded from equality calculations.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class ExcludeAttribute
	: Attribute
{
	/// <summary>
	/// Creates a new <see cref="ExcludeAttribute"/> instance.
	/// </summary>
	/// <param name="propertyNames">The properties to target.</param>
	public ExcludeAttribute(params string[] propertyNames) =>
		this.PropertyNames = propertyNames;

	/// <summary>
	/// Gets the names of the properties to target on a record.
	/// </summary>
   public string[] PropertyNames { get; }
}