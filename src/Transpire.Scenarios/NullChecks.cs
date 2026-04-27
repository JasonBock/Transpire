using System.Linq.Expressions;

namespace Transpire.Scenarios;

internal static class NullChecks
{
	internal static bool CheckEquality(string? value) => value == null;

	internal static bool CheckInequality(string? value) => value != null;

	internal static void CheckEqualityWithinExpression()
	{
		Expression<Func<string?, bool>> expression = value => value == null;
	}
}