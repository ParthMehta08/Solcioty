using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class Common
    {
    }
    public enum CommonResultsMessage
    {
        Add,
        Update,
        Fail,
        duplicate,

    }

    public static class ExtentionService
    {
        public static string ValidateFilename(this string input)
        {
            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9-]");
            return rgx.Replace(input, "");
        }
    }
}
