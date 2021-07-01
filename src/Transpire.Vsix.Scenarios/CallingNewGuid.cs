using System;

namespace Transpire.Vsix.Scenarios
{
	public static class CallingNewGuid
	{
		// This should be flagged as an error.
		public static Guid GetWithNewGuid() => new Guid();

		// This should be flagged as an error.
		public static Guid GetWithNewGuidTargetTypeNew() => new();

		public static Guid GetWithGuidNewGuid() => Guid.NewGuid();
	}
}