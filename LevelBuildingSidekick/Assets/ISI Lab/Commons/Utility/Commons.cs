using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.Commons
{
    public static class Commons
    {
        public static int indexFormat = 0;
        public static readonly string[] numberFormat = new string[] { " #", "_#", " (#)" };

        public static string CheckNameFormat(IEnumerable<string> context, string value, bool firstFormat = false, string prefix = "", string postfix = "", string specificFormat = "")        {
            var name = "";
            var loop = true;
            var v = 0;
            do
            {
                name = prefix + value + postfix;

                if (firstFormat)
                {
                    if(specificFormat == "")
                        name = prefix + value + postfix + numberFormat[indexFormat].Replace("#", v.ToString());
                    else
                        name = prefix + value + postfix + specificFormat.Replace("#", v.ToString());
                }

                firstFormat = true;

                if (!context.Contains(name))
                    loop = false;
                v++;
            } while (loop);

            return name;
        }
    }
}