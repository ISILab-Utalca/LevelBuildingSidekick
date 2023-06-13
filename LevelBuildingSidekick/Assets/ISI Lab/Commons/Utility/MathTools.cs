using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utility
{
    public static class MathTools
    {
        

        public static int GetAngleD15(Vector2 v1, Vector2 v2)
        {
            if(v1 == v2)
            {
                return -1;
            }
            float[] tgValues = { 0, 0.268f, 0.577f, 1, 1.732f, 3.732f };
            int[] tgAngles = { 0, 15, 30, 45, 60, 75 }; 
            int angle = 0;
            float dx = 0;
            Vector2 v = v1 - v2;
            if (v.x > 0 && v.y >= 0) //1st quadrant
            {
                angle = 0;
                dx = v.y / v.x;
            }
            else if (v.x <= 0 && v.y > 0)//2nd quadrante
            {
                angle = 90;
                dx = v.x / v.y;
            }
            else if (v.x < 0 && v.y <= 0)//3rd quadrant
            {
                angle = 180;
                dx = v.y / v.x;
            }
            else //if(v.x > 0 && v.y <= 0)
            {
                angle = 270;
                dx = v.x / v.y;
            }

            int index = 0;
            for (int i = 1; i < tgValues.Length; i++)
            {
                if(dx >= tgValues[i-1] && dx < tgValues[i])
                {
                    index = (dx - tgValues[i - 1]) < (tgValues[i] - dx) ? i - 1 : i;
                    break;
                }
            }

            return angle + tgAngles[index];
        }

        public static int GetAngleD90(Vector2 v1, Vector2 v2)
        {
            int angle = 0;
            Vector2 v = v1 - v2;
            if (v.x >= 0 && v.y > 0) //1st quadrant
            {
                angle = v.x > v.y ? 0 : 90; // tg < 1 <-> angle < 45, tg = v.y/v.x
            }
            else if (v.x < 0 && v.y >= 0)//2nd quadrante
            {
                angle = v.y > -v.x ? 90 : 180;
            }
            else if (v.x <= 0 && v.y < 0)//3rd quadrant
            {
                angle = -v.x > -v.y ? 180 : 270;
            }
            else //if(v.x > 0 && v.y <= 0)
            {
                angle = -v.y > v.x ? 270 : 0;
            }
            return angle;
        }

        public static T[,] ResizeArray<T>(T[,] original, int x, int y)
        {
            T[,] newArray = new T[x, y];
            int minX = Mathf.Min(original.GetLength(0), newArray.GetLength(0));
            int minY = Mathf.Min(original.GetLength(1), newArray.GetLength(1));

            for (int i = 0; i < minY; ++i)
                Array.Copy(original, i * original.GetLength(0), newArray, i * newArray.GetLength(0), minX);

            return newArray;
        }

        public static bool Compare2DArray<T>(T[,] first, T[,] second, out int differences)
        {
            bool different = false;
            differences = 0;
            int xDelta = 0;
            int yDelta = 0;
            int x = first.GetLength(0);
            if (x != second.GetLength(0))
            {
                xDelta = Mathf.Abs(second.GetLength(0) - x);
                x = x > second.GetLength(0) ? second.GetLength(0) : x;
                different = true;
            }
            int y = first.GetLength(1);
            if (y != second.GetLength(1))
            {
                yDelta = Mathf.Abs(second.GetLength(1) - y);
                y = y > second.GetLength(1) ? second.GetLength(1) : y;
                different = true;
            }


            for (int j = 0; j < y; j++)
            {
                for(int i = 0; i < x; i++)
                {
                    if(!first[x,y].Equals(second[x,y]))
                    {
                        different = true;
                        differences++;
                    }
                }
            }

            differences += yDelta * xDelta + yDelta * x + xDelta * y;

            return different;
        }
    }
}

