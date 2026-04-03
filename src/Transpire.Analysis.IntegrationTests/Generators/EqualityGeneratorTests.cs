using NUnit.Framework;

namespace Transpire.Analysis.IntegrationTests.Generators;

[Equality]
internal sealed partial record CustomerClassGeneric<T>(
	Guid Id, [property: Excluded] string Name, uint Age, T Address);

[Equality]
internal sealed partial record CustomerClassGeneriConstraint<T>(
	Guid Id, [property: Excluded] string Name, uint Age, T Address) where T : struct;

[Equality]
internal sealed partial record CustomerClassExcluded(
	Guid Id, [property: Excluded] string Name, uint Age);

[Equality]
internal partial record struct CustomerStructExcluded(
	Guid Id, [property: Excluded] string Name, uint Age);

[Equality]
internal sealed partial record Order(
	[property: Ordered(1)] Content content1, [property: Ordered(0)] Content content0);

[Equality]
internal sealed partial record OrderAndExclude(
	[property: Excluded] string Name, [property: Ordered(1)] Content content1, [property: Ordered(0)] Content content0);

internal sealed class Ordering
{
	internal int Value { get; set; }
}

[Equality]
internal abstract partial record AbstractCustomer(
	Guid Id, [property: Excluded] string Name, [property: Ordered(3)] uint Age);

internal sealed partial record ConcreteCustomer(
	Guid Id, string Name, uint Age, string Address) 
	: AbstractCustomer(Id, Name, Age);

internal sealed partial class OuterCustomer
{
	[Equality]
	internal sealed partial record InternalCustomer(
		Guid Id, [property: Excluded] string Name, [property: Ordered(3)] uint Age);
}

internal sealed class Content
	: IEquatable<Content>
{
	internal Content(Ordering ordering) =>
		this.Ordering = ordering;

	private Ordering Ordering { get; }

	public bool Equals(Content? other)
	{
		this.OrderingValue = this.Ordering.Value;
		this.Ordering.Value++;
		return true;
	}

	internal int OrderingValue { get; private set; }

	public override bool Equals(object? obj) =>
		this.Equals(obj as Content);

	public override int GetHashCode() => base.GetHashCode();
}

internal static class EqualityGeneratorTests
{
	[Test]
	public static void CheckEqualityOnClassWithExcluded()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new CustomerClassExcluded(id, "Jane", age);
		var customer2 = new CustomerClassExcluded(id, "Joe", age);

		Assert.That(customer2, Is.EqualTo(customer1));
	}

	[Test]
	public static void CheckEqualityOnClassWithOrdered()
	{
		var ordering2 = new Ordering();

		var content2_1 = new Content(ordering2);
		var content2_0 = new Content(ordering2);

		var order1 = new Order(new Content(new Ordering()), new Content(new Ordering()));
		var order2 = new Order(content2_1, content2_0);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(order1, Is.EqualTo(order2));
			Assert.That(content2_0.OrderingValue, Is.Zero);
			Assert.That(content2_1.OrderingValue, Is.EqualTo(1));
		}
	}

	[Test]
	public static void CheckEqualityOnClassWithOrderedAndExcluded()
	{
		var ordering2 = new Ordering();

		var content2_1 = new Content(ordering2);
		var content2_0 = new Content(ordering2);

		var order1 = new OrderAndExclude("Jane", new Content(new Ordering()), new Content(new Ordering()));
		var order2 = new OrderAndExclude("Joe", content2_1, content2_0);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(order1, Is.EqualTo(order2));
			Assert.That(content2_0.OrderingValue, Is.Zero);
			Assert.That(content2_1.OrderingValue, Is.EqualTo(1));
		}
	}

	[Test]
	public static void CheckEqualityOnGeneric()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new CustomerClassGeneric<string>(id, "Jane", age, "123 Street");
		var customer2 = new CustomerClassGeneric<string>(id, "Joe", age, "123 Street");

		Assert.That(customer2, Is.EqualTo(customer1));
	}

	[Test]
	public static void CheckEqualityOnGenericConstraint()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new CustomerClassGeneriConstraint<int>(id, "Jane", age, 123);
		var customer2 = new CustomerClassGeneriConstraint<int>(id, "Joe", age, 123);

		Assert.That(customer2, Is.EqualTo(customer1));
	}

	[Test]
	public static void CheckEqualityOnAbstract()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new ConcreteCustomer(id, "Jane", age, "123 Street");
		var customer2 = new ConcreteCustomer(id, "Joe", age, "123 Street");

		Assert.That(customer2, Is.EqualTo(customer1));
	}

	[Test]
	public static void CheckEqualityOnNested()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new OuterCustomer.InternalCustomer(id, "Jane", age);
		var customer2 = new OuterCustomer.InternalCustomer(id, "Joe", age);

		Assert.That(customer2, Is.EqualTo(customer1));
	}

	[Test]
	public static void CheckEqualityOnStruct()
	{
		var id = Guid.NewGuid();
		var age = 22u;

		var customer1 = new CustomerStructExcluded(id, "Jane", age);
		var customer2 = new CustomerStructExcluded(id, "Joe", age);

		Assert.That(customer2, Is.EqualTo(customer1));
	}
}