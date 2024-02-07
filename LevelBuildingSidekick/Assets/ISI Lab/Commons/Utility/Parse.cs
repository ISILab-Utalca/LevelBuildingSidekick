using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace ISILab.Commons.Utility
{
    public static class Parse
    {
        /// <summary>
        /// Parce string to Color.
        /// </summary>
        /// <returns></returns>
        public static Color StrToColor(string s)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + s, out c))
            {
                return c;
            }
            return Color.magenta;
        }

        /// <summary>
        /// Parse color to string.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ColorTosStr(Color c)
        {
            return ColorUtility.ToHtmlStringRGB(c);
        }
    }
}