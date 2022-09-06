using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class Parse
{
    /// <summary>
    /// Get color from string whit "#f0f0f0f0" -> #RGBA format
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

    public static string ColorTosStr(Color c)
    {
        return ColorUtility.ToHtmlStringRGB(c);
    }

}

public static class Watch
{
    private static Dictionary<string,Stopwatch> watches = new Dictionary<string,Stopwatch>();

    public static void Start(string label = "")
    {
        watches[label] = Stopwatch.StartNew();
    }

    public static void End(string label ="")
    {
        Stopwatch w;
        if (watches.TryGetValue(label, out w))
        {
            w.Stop();
            var elapsedMs = w.ElapsedMilliseconds;
            UnityEngine.Debug.Log(label+": " + elapsedMs);
            watches.Remove(label);
        }
        else
        {
            UnityEngine.Debug.Log("No existe reloj con la label '"+label+"'.");
        }
    }

}


