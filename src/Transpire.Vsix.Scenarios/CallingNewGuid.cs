using System;

namespace Transpire.Vsix.Scenarios
{
	public static class CallingNewGuid
	{
		// This should be flagged as an error.
		public static Guid GetWithNewGuid() => new Guid();

		public static Guid GetWithGuidNewGuid() => Guid.NewGuid();
	}
}