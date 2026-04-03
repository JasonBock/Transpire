There's essentially two methods to implement, given a type like this:

```c#
public sealed record Person(Guid Id, uint Age, string Name);
```

```c#
public bool Equals(Person other)
{
	return (object)this == other || 
		((object)other != null && 
			this.EqualityContract == other.EqualityContract && 
			global::System.Collections.Generic.EqualityComparer<global::System.Guid>.Default.Equals(this.Id, other.Id) && 
			global::System.Collections.Generic.EqualityComparer<uint>.Default.Equals(this.Age, other.Age) && 
			global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(this.Name, other.Name));
}

public override int GetHashCode()
{
    var hash = new global::System.HashCode();
    hash.Add(this.Id);
    hash.Add(this.Age);
    hash.Add(this.Name);
    return hash.ToHashCode();
}
```

Design #2:

```c#
[Equality]
public record Customer([property: Equality(3)] string Name)
{
  public Customer(Guid id)
    : this("Joe") => this.Id = id;

  [Equality(RecordUsage.Excluded)]
  public uint Age { get; }

  [Equality(4)]
  public Guid Id { get; }
}

public enum RecordUsage { Included, Excluded }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class EqualityAttribute
  : Attribute { }

[AttributeUsage(AttributeTargets.Property)]
public sealed class EqualityAttribute
  : Attribute
{
  public EqualityAttribute(uint order) => this.Order = order;
  public EqualityAttribute(RecordUsage usage) => this.Usage = usage;
  public EqualityAttribute(RecordUsage usage, uint order) => 
    (this.Usage, this.Order) = (usage, order);

  public RecordUsage Usage { get; }
  public uint? Order { get; }
}
```

Fixes
* It's outputting `struct` and it shouldn't
* The properties should always exclude `EqualityContract`

    // Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs(10,5): error CS8600: Converting null literal or possible null value to non-nullable type.
    DiagnosticResult.CompilerError("CS8600").WithSpan("Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs", 10, 5, 10, 18),

Success!

    // /0/Test0.cs(11,25): error CS9034: Required member 'Customer.Name' must be settable.
    DiagnosticResult.CompilerError("CS9034").WithSpan(11, 25, 11, 29).WithArguments("Customer.Name"),

    // Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs(10,96): error CS1002: ; expected
    DiagnosticResult.CompilerError("CS1002").WithSpan("Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs", 10, 96, 10, 97),
    // Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs(10,96): error CS1519: Invalid token ')' in a member declaration
    DiagnosticResult.CompilerError("CS1519").WithSpan("Transpire.Analysis\Transpire.Analysis.EqualityGenerator\Customer_Equality.g.cs", 10, 96, 10, 97).WithArguments(")"),



# TODO
* DONE - Tests cases:
  * DONE - When record is `struct`, should change `Equals()` to not be `virtual`, `struct` added to definition, and `Equals` should not have `?` for the parameter
  * DONE - When `[Equality]` doesn't exist, nothing should be done
  * DONE - Excluding and ordered declared property
  * DONE - Multiple sorting and excluding
  * DONE - Generic records
    * DONE - With constraints
  * DONE - Nullable properties
  * DONE - Putting record in namespace
  * DONE - Accessibility - e.g. `internal`
  * DONE - Sealed record
  * DONE - Abstract record
  * DONE - Nested record
  * DONE - Integration tests
* Diagnostics (note that the first bullet point is not covered by the diagnostic analysis in the source generator):
  * If `[Equality]` does not exist
    * If `[Excluded]` or `[Ordered]` exist on a property, error
  * If `[Equality]` exists
    * On a non-record, error
    * On a record that isn't partial, error
    * Both `[Excluded]` and `[Ordered]` exist on a property, error
    * The type doesn't have any properties marked with either `[Excluded]` or `[Ordered]`, error
    * If every property ends up being excluded, error
    * If there's only one property, and it has `[Ordered]`, error
