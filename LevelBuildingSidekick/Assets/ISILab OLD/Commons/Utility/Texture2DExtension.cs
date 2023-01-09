using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utility
{
    public static class Texture2DExtension
    {
        public static void InsertTexture(this Texture2D origin, Texture2D other, int x, int y)
        {
            origin.SetPixels(x, y, other.width, other.height, other.GetPixels());
            origin.Apply();
        }

        public static void InsertTextureInRect(this Texture2D origin, Texture2D other, int x, int y, int width, int height)
        {
            var pixels = Resizer.Resize2DArray(other.GetPixels(), other.width, other.height, width, height);
            try
            {
                /*if (x + width > origin.width || x + width > other.width || y + height > origin.height || y + height > other.height)
                {
                    return;
                }*/

                origin.SetPixels(x, y, width, height, pixels);
                origin.Apply();
            }
            catch
            {
                Debug.LogError("Process not completed, image out of bounds" +
                    " OriginW: " + origin.width + " - OriginH: " + origin.height + " - OtherW: " + other.width + " - OtherH: " + other.height
                    + " - TW: " + width + " - TH: " + height + " - pixels: " + pixels.Length + " - X: " + x + " - Y: " + y);
            }
            
        }

        public static void MirrorY(this Texture2D origin)
        {
            int height = origin.height;
            Color[] pixels = new Color[origin.width*origin.height];
            Array.Copy(origin.GetPixels(),pixels, pixels.Length);

            for(int i = 0; i < height; i++)
            {
                origin.SetPixels(0, i, origin.width, 1, pixels.Skip((height - 1 - i) * origin.width).Take(origin.width).ToArray());
            }
            origin.Apply();
        }

        public static void MirrorY(this Texture2D origin, int stride)
        {
            int height = origin.height/stride;
            Color[] pixels = new Color[origin.width * origin.height];
            Array.Copy(origin.GetPixels(), pixels, pixels.Length);

            for (int i = 0; i < height; i++)
            {
                origin.SetPixels(0, i, origin.width, stride, pixels.Skip((height - 1 - i) * origin.width).Take(origin.width*stride).ToArray());
            }
            origin.Apply();
        }

        public static Texture2D Merge(this Texture2D origin, Texture2D other)
        {
            if(origin == null || other == null)
            {
                return origin;
            }

            if(origin.width != other.width || origin.height != other.height)
            {
                return origin;
            }

            int width = origin.width < other.width ? origin.width : other.width;
            int height = origin.height < other.height ? origin.height : other.height;

            var t = new Texture2D(origin.width, origin.height);

            for(int j = 0; j < origin.height; j++)
            {
                for (int i = 0; i < origin.width; i++)
                {
                    if(other.GetPixel(i,j).a == 0)
                    {
                        t.SetPixel(i, j, origin.GetPixel(i, j));
                    }
                    else
                    {
                        t.SetPixel(i, j, other.GetPixel(i, j));
                    }
                }
            }
            t.Apply();
            return t;
        }
    }
}

