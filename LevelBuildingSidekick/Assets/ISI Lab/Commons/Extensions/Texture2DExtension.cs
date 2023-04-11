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

        public static Texture2D MergeTextures(this Texture2D origin, Texture2D other)
        {
            if (origin.width != other.width || origin.height != other.height)
            {
                throw new Exception("Textures do not have the same size");
            }

            // Create a new texture to hold the merged result
            Texture2D mergedTexture = new Texture2D(origin.width, origin.height, TextureFormat.RGBA32, false);

            // Copy the contents of the first texture into the new texture
            Graphics.CopyTexture(origin, 0, 0, mergedTexture, 0, 0);

            // Copy the contents of the second texture into the new texture, with blending
            Graphics.CopyTexture(other, 0, 0, mergedTexture, 0, 0);

            // Apply the changes to the new texture
            mergedTexture.Apply();

            return mergedTexture;
        }
    }
}

