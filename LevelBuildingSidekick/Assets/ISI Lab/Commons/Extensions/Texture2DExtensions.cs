using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ISILab.Commons.Utility;

namespace Utility
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Insert a texture in the other texture.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="other"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void InsertTexture(this Texture2D origin, Texture2D other, int x, int y)
        {
            origin.SetPixels(x, y, other.width, other.height, other.GetPixels());
            origin.Apply();
        }

        /// <summary>
        /// Insert a texture in a rect of the other texture.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="other"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void InsertTextureInRect(this Texture2D origin, Texture2D other, int x, int y, int width, int height)
        {
            var pixels = Resizer.Resize2DArray(other.GetPixels(), other.width, other.height, width, height);
            try
            {
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

        /// <summary>
        /// Mirror the texture in the X axis.
        /// </summary>
        /// <param name="origin"></param>
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

        /// <summary>
        /// Mirror the texture in the Y axis.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="stride"></param>
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

        /// <summary>
        /// Merge two textures, the second one will be the mask.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// Fit the texture in a square.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return a subtexture from the original texture.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Set all pixels of the texture to a color.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="color"></param>
        public static void SetAllPixels(this Texture2D texture, Color32 color)
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

