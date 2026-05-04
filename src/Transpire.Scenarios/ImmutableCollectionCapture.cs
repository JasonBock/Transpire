using System.Collections.Immutable;

namespace Transpire.Scenarios;

internal static class ImmutableCollectionCapture
{
	internal static int NewCount()
	{
		ImmutableList<int> items = [2, 3, 4];
		items.Add(5);
		return items.Count;
	}
}