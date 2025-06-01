TODO:

* `DiscourageNonGenericCollectionTypeDeclarationUsageAnalyzer` - stops people from doing `public class Stuff : ArrayList { ... }`. We want to catch those concrete types used in inheritance hierarchies.
* Get rid of putting `using Verify = CSharpAnalyzerVerifier<FindNewDateTimeViaConstructorAnalyzer, DefaultVerifier>;` in every test.
* Put in an issue to ensure all Transpire documentation is updated