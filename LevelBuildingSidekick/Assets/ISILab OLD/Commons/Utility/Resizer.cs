using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utility
{
    public static class Resizer
    {
        public static T[] Resize2DArray<T>(T[] data, int srcWidth, int srcHeight, int dstWidth, int dstheight)
        {
            T[] temp = new T[dstWidth * dstheight];
            // EDIT: added +1 to account for an early rounding problem
            int x_ratio = (int)((srcWidth << 16) / dstWidth) + 1;
            int y_ratio = (int)((srcHeight << 16) / dstheight) + 1;
            //int x_ratio = (int)((w1<<16)/w2) ;
            //int y_ratio = (int)((h1<<16)/h2) ;
            int x2, y2;
            for (int i = 0; i < dstheight; i++)
            {
                for (int j = 0; j < dstWidth; j++)
                {
                    x2 = ((j * x_ratio) >> 16);
                    y2 = ((i * y_ratio) >> 16);
                    temp[(i * dstWidth) + j] = data[(y2 * srcWidth) + x2];
                }
            }
            return temp;
        }

        public static T[] Resize<T>(this T[] array, int size)
        {
            var newarray = new T[size];
            var length = size > array.Length ? array.Length : size;
            Array.Copy(array, newarray, length);
            return newarray;
        }
    }

}

