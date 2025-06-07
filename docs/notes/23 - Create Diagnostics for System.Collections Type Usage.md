 `DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer` - stops people from doing `public class Stuff : ArrayList { ... }`. We want to catch those concrete types used in inheritance hierarchies.
* What other cases?
    * Types in constraints (both type and method type parameters): `where T : ArrayList`
    * Parameter types (and return types): `public ArrayList Work(ArrayList data) { }`
    * Property types: `public ArrayList Customer { get; }`
    * Field types: `private readonly ArrayList customers = new();`
* Maybe make one uber-analyzer, and register all the work within that one.

* Get rid of putting `using Verify = CSharpAnalyzerVerifier<FindNewDateTimeViaConstructorAnalyzer, DefaultVerifier>;` in every test.

Done with everything!