using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Solcioty.utilities
{
	public static class Extension
	{
		public static string LimitedDisplayNote(this string stringvalue,int column)
		{
			var limitedDisplayNote = string.Empty;
			if (column == 2 && stringvalue.Length > 16)
				limitedDisplayNote = stringvalue.Substring(0, 16);
			else if (column == 3 && stringvalue.Length > 14)
				limitedDisplayNote = stringvalue.Substring(0, 14);
			else if (column == 4 && stringvalue.Length > 12)
				limitedDisplayNote = stringvalue.Substring(0, 12);
			else if (column == 5 && stringvalue.Length > 10)
				limitedDisplayNote = stringvalue.Substring(0, 10);
			else
				limitedDisplayNote = stringvalue;
			return limitedDisplayNote;
		}

		public static string LimitedDisplayClass(this string stringvalue, int column)
		{
			var limitedDisplayClass = string.Empty;
			//if (column == 1)
			//	limitedDisplayClass = $"repslabel-{column}";
			//else if (column == 2 && stringvalue.Length > 16)
			//	limitedDisplayClass = $"repslabel-{column}";
			//else if (column == 3 && stringvalue.Length > 14)
			//	limitedDisplayClass = $"repslabel-{column}";
			//else if (column == 4 && stringvalue.Length > 12)
			//	limitedDisplayClass = $"repslabel-{column}";
			//else if (column == 5 && stringvalue.Length > 10)
			//	limitedDisplayClass = $"repslabel-{column}";
			if (string.IsNullOrEmpty(stringvalue)) {
				return $"repslabel-default-{column}";
			}
			if (column == 1)
				limitedDisplayClass = $"repslabel-{column}";
			else if (column == 2 && stringvalue.Length > 11)
				limitedDisplayClass = $"repslabel-{column}";
			else if (column == 3 && stringvalue.Length > 11)
				limitedDisplayClass = $"repslabel-{column}";
			else if (column == 4 && stringvalue.Length > 11)
				limitedDisplayClass = $"repslabel-{column}";
			else if (column == 5 && stringvalue.Length > 11)
				limitedDisplayClass = $"repslabel-{column}";
			else
				limitedDisplayClass = $"repslabel-default-{column}";
			return limitedDisplayClass;
		}

	}
}