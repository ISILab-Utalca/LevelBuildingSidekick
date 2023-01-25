using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension
{
    public static Color RandomColor(this Color color)
    {
        color = new Color(Random.Range(0f,255f), Random.Range(0f, 255f), Random.Range(0f, 255f));
        return color;
    }

    public static Color RandomGrayScale(this Color color)
    {
        var gray = Random.Range(0f, 255f);
        color = new Color(gray, gray, gray);
        return color;
    }
}
