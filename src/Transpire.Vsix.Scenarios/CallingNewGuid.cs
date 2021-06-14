using System;

namespace Transpire.Vsix.Scenarios
{
	public static class CallingNewGuid
	{
		public static Guid GetWithNewGuid() => new Guid();

		public static Guid GetWithGuidNewGuid() => Guid.NewGuid();
	}
}