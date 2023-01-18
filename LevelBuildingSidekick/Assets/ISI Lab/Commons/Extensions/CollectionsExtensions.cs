using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionsExtensions
{
    #region LIST

    public static bool ContainsIndex<T>(this List<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    #endregion

    #region Array

    public static bool ContainsIndex<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    #endregion

}
