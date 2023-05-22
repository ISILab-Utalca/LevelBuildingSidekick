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
    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count <= 0)
        {
            Debug.Log("[ISI Lab]: Error to try get a random element in '" + list + "' because is empty.");
            return default(T);
        }

        return list[UnityEngine.Random.Range(0, list.Count - 1)];
    }

    #endregion

    #region Array

    public static bool ContainsIndex<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    public static T GetRandom<T>(this T[] array)
    {
        if (array.Length <= 0)
        {
            Debug.Log("[ISI Lab]: Error to try get a random element in '"+array+"' because is empty.");
            return default(T);
        }

        return array[UnityEngine.Random.Range(0,array.Length - 1)];
    }

    #endregion

}
