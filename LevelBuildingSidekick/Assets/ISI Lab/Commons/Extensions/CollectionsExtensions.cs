using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uRandom = UnityEngine.Random;

public static class CollectionsExtensions
{
    #region LIST
    public static List<T> Clone<T>(this List<T> list) where T : class
    {
        var clone = new List<T>();

        foreach (var item in list)
        {
            if(item is ICloneable)
            {
                var c = (item as ICloneable).Clone() as T;
                clone.Add(c);
            }
            else
            {
                Debug.LogWarning("Item: '"+item+"' in '"+list+"' cannot be cloned.");
                clone.Add(item);
            }
        }
        return clone;
    }

    public static T RandomRullete<T>(this List<T> list,Func<T,float> predicate)
    {
        if(list.Count <= 0)
        {
            return default(T);
        }

        var pairs = new List<Tuple<T, float>>();
        for (int i = 0; i < list.Count(); i++)
        {
            var value = predicate(list[i]);
            pairs.Add(new Tuple<T, float>(list[i], value));
        }

        var total = pairs.Sum(p => p.Item2);
        var rand = uRandom.Range((float)0, total);

        var cur = 0f;
        for (int i = 0; i < pairs.Count; i++)
        {
            cur += pairs[i].Item2;
            if (rand <= cur)
            {
                return pairs[i].Item1;
            }
        }
        return default(T);
    }

    public static bool ContainsIndex<T>(this List<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    public static T Random<T>(this List<T> list)
    {
        if (list.Count <= 0)
        {
            Debug.Log("[ISI Lab]: Error to try get a random element in '" + list + "' because is empty.");
            return default(T);
        }

        return list[UnityEngine.Random.Range(0, list.Count - 1)];
    }

    public static List<T> Rotate<T>(this List<T> list, int count)
    {
        if (count <= 0)
            return list;

        var c = count % list.Count;
        int rotationIndex = list.Count - c;
        List<T> rotatedList = new List<T>();

        for (int i = rotationIndex; i < list.Count; i++)
            rotatedList.Add(list[i]);

        for (int i = 0; i < rotationIndex; i++)
            rotatedList.Add(list[i]);

        return rotatedList;
    }

    public static List<T> Rotate<T>(this List<T> list)
    {
        var toR = new List<T>(list);

        if (toR.Count <= 0)
            return toR;

        var temp = toR.Last();
        toR.Remove(temp);
        toR.Insert(0, temp);

        return toR;
    }

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
