using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StringExtensions
{
    private static string[] Rotate(this string[] group)
    {
        var temp = group.ToList();
        var last = group.Last();
        temp.RemoveAt(temp.Count - 1);
        var r = new List<string>() { last };
        r.AddRange(temp);

        var toR = new string[4];
        for (int i = 0; i < 4; i++)
        {
            toR[i] = r[i];
        }

        return toR;
    }
}
