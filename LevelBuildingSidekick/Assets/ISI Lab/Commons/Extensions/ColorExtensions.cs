using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ColorExtensions
{
    public static Color RandomColor(this Color color)
    {
        color = new Color(
            Random.Range(0f, 255f) / 255f,
            Random.Range(0f, 255f) / 255f,
            Random.Range(0f, 255f) / 255f);
        return color;
    }

    public static Color RandomGrayScale(this Color color)
    {
        var gray = Random.Range(0f, 255f) / 255f;
        color = new Color(gray, gray, gray);
        return color;
    }

    public static Color Inverse(this Color color)
    {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b);
    }
}