using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class MathTools
    {
        public static float PointToLineDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {

            float dist = Vector2.Distance(lineStart, lineEnd);
            if (dist == 0)
            {
                return Vector2.Distance(point, lineStart);
            }

            /*float m = lineEnd.y - lineStart.y / (lineEnd.x - lineStart.x);

            Vector2 p1 = new Vector2(point.x, m * point.x);
            Vector2 p2 = new Vector2(point.y / m, point.y);*/


            float perc = Vector2.Dot((point - lineStart)/dist, (lineEnd - lineStart)/dist);
            
            Vector2 v = lineStart + ((lineEnd - lineStart) * perc);

            //Debug.Log("S: " + lineStart + " - E: " + lineEnd + " - P:" + point + " - Perc:" + perc + " - P_SE: " + v + " - D: " + Vector2.Distance(v, point));

            return Vector2.Distance(v,point);
        }

        /*public static int GetAngleD15(Vector2 v1, Vector2 v2)
        {
            float[] tgValues = { 0, 0.268f, 0.577f, 1, 1.732f, 3.732f };
            int angle = 0;
            Vector2 v = v1 - v2;
            if(v.x <= 0 && v.y > 0)
            {
                angle = 90;
            }
            else if(v.x < 0 && v.y <= 0)
            {
                angle = 180;
            }
            else if(v.x >= 0 && v.y < 0)
            {
                angle = 270;
            }

            for(int i = 0; i < tgValues.Length; i++)
            {

            }

            return angle;
        }*/

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
    }
}

