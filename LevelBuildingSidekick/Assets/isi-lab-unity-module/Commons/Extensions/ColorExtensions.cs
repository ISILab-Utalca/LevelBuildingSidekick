using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace ISILab.Extensions
{
    public static class ColorExtensions
    {
        private const float colorDifferenceValue = 0.25f;
        private static List<Color> recentColors = new List<Color>();
        
        // 😫 Should either improve the value generation or
        // create a dictionary of colors and apply small modifications on it
        //
        // （づ￣3￣）づ╭🖌️～
        public static Color RandomColorHSV(this Color color)
        {

            do
            {
                float hue = Random.Range(0f, 1f);
                float saturation = Random.Range(0.75f, 1f);
                float value = Random.Range(0.75f, 1f);
                color = Color.HSVToRGB(hue, saturation, value);
            }while(!ColorDifferentEnough(color));
            
            if(recentColors.Count>5) recentColors.RemoveAt(0);
            recentColors.Add(color);
            
            return color;
        }

        private static bool ColorDifferentEnough(Color newColor)
        {
            if (recentColors.Count == 0) return true;
            
            foreach (var savedColor in recentColors)
            {
                var redDiff = Mathf.Abs(savedColor.r - newColor.r);
                var greenDiff = Mathf.Abs(savedColor.g - newColor.g);
                var blueDiff = Mathf.Abs(savedColor.b - newColor.b);
                if (redDiff < colorDifferenceValue &&
                    greenDiff <  colorDifferenceValue &&
                    blueDiff < colorDifferenceValue)
                {
                   
                    return false;
                }
            }
            return true;
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