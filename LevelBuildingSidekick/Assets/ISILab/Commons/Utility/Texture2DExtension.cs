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
            origin.SetPixels(x, y, width, height, pixels);
            origin.Apply();
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
            if(origin.width != other.width || origin.height != other.height)
            {
                Debug.LogError("Textures have different sizes");
                return null;
            }

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

