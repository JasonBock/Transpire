namespace Transpire;

/// <summary>
/// Indicates that a class or struct supports custom equality markup for code generation or tooling purposes.
/// </summary>
/// <remarks>
/// Apply this attribute to a record to signal that it participates in equality-related code
/// generation or analysis. This attribute is typically used by source generators or tools that require explicit
/// identification of types with custom equality semantics.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EqualityAttribute
  : Attribute
{ }