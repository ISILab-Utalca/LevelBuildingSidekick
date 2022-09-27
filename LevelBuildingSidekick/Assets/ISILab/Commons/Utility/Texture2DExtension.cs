using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class Texture2DExtension
    {
        public static void InsertTexture(this Texture2D origin, Texture2D other, int x, int y)
        {
            origin.SetPixels(x, y, other.width, other.height, other.GetPixels());
        }

        public static void InsertTextureInRect(this Texture2D origin, Texture2D other, int x, int y, int width, int height)
        {
            var pixels = Resizer.Resize2DArray(other.GetPixels(), other.width, other.height, width, height);
            origin.SetPixels(x, y, width, height, pixels);
        }
    }
}

