using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utility
{
    public static class Texture2DExtensions
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

        public static Texture2D MergeTextures(this Texture2D origin, Texture2D other)
        {
            if (origin.width != other.width || origin.height != other.height)
            {
                throw new Exception("Textures do not have the same size");
            }

            var t = new Texture2D(origin.width, origin.height);

            for (int j = 0; j < origin.height; j++)
            {
                for (int i = 0; i < origin.width; i++)
                {
                    if (other.GetPixel(i, j).a == 0)
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

        public static Texture2D FitSquare(this Texture2D origin)
        {

            int size = (origin.width > origin.height ? origin.width : origin.height);
            var ofsset = ((Vector2.one * size - new Vector2(origin.width, origin.height))/2).ToInt();
            var texture = new Texture2D(size, size);

            for (int j = 0; j < origin.height; j++)
            {
                for (int i = 0; i < origin.width; i++)
                {
                    texture.SetPixel(ofsset.x + i, ofsset.y + j, origin.GetPixel(i,j));
                }
            }
            texture.Apply();

            return texture;
        }

        public static Texture2D SubTexture(this Texture2D origin, int x, int y, int width, int height)
        {
            var texture = new Texture2D(width, height);

            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    texture.SetPixel(i, j, origin.GetPixel(x + i, y + j));
                }
            }

            texture.Apply();

            return texture;
        }

        public static void Set(this Texture2D texture, Color32 color)
        {
            for(int j = 0; j < texture.height; j++)
            {
                for (int i = 0; i < texture.width; i++)
                {
                    texture.SetPixel(i,j, color);
                }
            }

            texture.Apply();
        }
    }
}

