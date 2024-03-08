using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ISILab.Commons.Utility
{
    public static class Resizer
    {
        /// <summary>
        /// Resize a 2D array to a new size using bilinear interpolation algorithm.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="srcWidth"></param>
        /// <param name="srcHeight"></param>
        /// <param name="dstWidth"></param>
        /// <param name="dstHeight"></param>
        /// <returns></returns>
        public static T[] Resize2DArray<T>(T[] data, int srcWidth, int srcHeight, int dstWidth, int dstHeight)
        {
            T[] temp = new T[dstWidth * dstHeight];
            int xRatio = (int)((srcWidth << 16) / dstWidth) + 1; // EDIT: added +1 to account for an early rounding problem
            int yRatio = (int)((srcHeight << 16) / dstHeight) + 1;
            for (int i = 0; i < dstHeight; i++)
            {
                for (int j = 0; j < dstWidth; j++)
                {
                    var x = ((j * xRatio) >> 16);
                    var y = ((i * yRatio) >> 16);
                    temp[(i * dstWidth) + j] = data[(y * srcWidth) + x];
                }
            }
            return temp;
        }

        /// <summary>
        /// Resize a 2D array to a new size.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static T[] Resize<T>(this T[] array, int size)
        {
            var newarray = new T[size];
            var length = size > array.Length ? array.Length : size;
            Array.Copy(array, newarray, length);
            return newarray;
        }
    }
}