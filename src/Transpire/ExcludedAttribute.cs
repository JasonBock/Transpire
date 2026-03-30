namespace Transpire;

/// <summary>
/// Specifies that a property should be excluded from equality operations on a record.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ExcludedAttribute
	: Attribute
{ }