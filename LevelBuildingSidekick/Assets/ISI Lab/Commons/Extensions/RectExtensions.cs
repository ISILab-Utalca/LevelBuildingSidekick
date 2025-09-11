using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.Extensions
{
    public static class RectExtensions
    {
        /// <summary>
        /// Gets the minimum and maximum x and y axis points of the two rectangles (forming the area), 
        /// and applies said area to itself.
        /// </summary>
        public static void GetCombinedArea(this ref Rect rect, Rect a, Rect b)
        {
            rect.xMin = Mathf.Min(a.xMin, b.xMin);
            rect.xMax = Mathf.Max(a.xMax, b.xMax);
            rect.yMin = Mathf.Min(a.yMin, b.yMin);
            rect.yMax = Mathf.Max(a.yMax, b.yMax);
        }

        /// <summary>
        /// Gets the minimum and maximum x and y axis points of it's own rectangle and another (forming the area), 
        /// and applies said area to itself.
        /// </summary>
        public static void GetCombinedArea(this ref Rect rect, Rect b)
        {
            rect.xMin = Mathf.Min(rect.xMin, b.xMin);
            rect.xMax = Mathf.Max(rect.xMax, b.xMax);
            rect.yMin = Mathf.Min(rect.yMin, b.yMin);
            rect.yMax = Mathf.Max(rect.yMax, b.yMax);
        }
    }
}
