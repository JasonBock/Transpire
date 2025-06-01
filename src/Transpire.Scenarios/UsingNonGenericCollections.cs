using System.Collections;

namespace Transpire.Scenarios;

public static class UsingNonGenericCollections
{
	public static int SumGenericCounts()
	{
		var listCollection = new List<string>();
		var queueCollection = new Queue<string>();

		return listCollection.Count +
			queueCollection.Count;
	}

	public static int SumNonGenericCounts()
	{
		var arrayListCollection = new ArrayList();
		var queueCollection = new Queue();

		return arrayListCollection.Count + 
			queueCollection.Count;
	}
}