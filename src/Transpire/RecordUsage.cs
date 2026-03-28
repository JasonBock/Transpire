namespace Transpire;

/// <summary>
/// Specifies whether a property is included or excluded in equality operations for a record.
/// </summary>
public enum RecordUsage 
{ 
	/// <summary>
	/// The property is included.
	/// </summary>
	Included,
	/// <summary>
	/// The property is excluded.
	/// </summary>
	Excluded
}