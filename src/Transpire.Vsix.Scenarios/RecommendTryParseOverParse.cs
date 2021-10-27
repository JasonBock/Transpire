using System;
using System.Collections.Generic;
using System.Text;

namespace Transpire.Vsix.Scenarios
{
	public static class RecommendTryParseOverParse
	{
		public static int UseTryParse(string data)
		{
			var dataResult = int.TryParse(data, out var result);

			if (dataResult)
			{
				return result * result;
			}
			else
			{
				return 0;
			}
		}

		public static int UseParse(string data)
		{
			var result = int.Parse(data);
			return result * result;
		}
	}
}
