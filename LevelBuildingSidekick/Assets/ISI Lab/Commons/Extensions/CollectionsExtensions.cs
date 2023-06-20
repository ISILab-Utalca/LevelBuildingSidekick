using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public static List<T> Rotate<T>(this List<T> list)
    {
        var toR = new List<T>(list);

        if (toR.Count <= 0)
            return toR;

        var temp = toR.Last();
        toR.Remove(temp);
        toR.Add(temp);

        return toR;
    }

    /*
    private static T[] Rotate<T>(this T[] array)
    {
        if (array.Length <= 0)
            return array;

        var temp = array.ToList();
        var last = array.Last();
        temp.RemoveAt(temp.Count - 1);
        var r = new List<T>() { last };
        r.AddRange(temp);

        var toR = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            toR[i] = r[i];
        }

        return toR;
    }
    */

    public static List<T> RemoveEmpties<T>(this List<T> list)
    {
        list = list.Where(b => b != null).ToList();
        return list;
    }

    public static List<T> RemoveDuplicates<T>(this List<T> list)
    {
        var toR = new List<T>();
        foreach (var item in list)
        {
            if (!toR.Contains(item))
                toR.Add(item);
        }
        return toR;
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
