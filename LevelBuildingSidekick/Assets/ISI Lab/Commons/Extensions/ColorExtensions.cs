using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ISILab.Extensions
{
    public static class ColorExtensions
    {
        public static Color RandomColorHSV(this Color color)
        {
            float hue = Random.Range(0f, 1f); 
            float saturation = 1f;            
            float value = 1f;    // Closer to yellow, lower value, the closer to blue, higher value              
            color = Color.HSVToRGB(hue, saturation, value);

            return color;
        }
        public static Color RandomColorRGB(this Color color)
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
}