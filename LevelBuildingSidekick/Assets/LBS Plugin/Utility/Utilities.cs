using LevelBuildingSidekick.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void Print(this SchemaData schema)
    {
        string[,] m = schema.GetMatrix();
        string msg = "";
        for (int i = 0; i < m.GetLength(0); i++)
        {
            for (int j = 0; j < m.GetLength(1); j++)
            {
                if(m[i,j] != "")
                {
                    msg += "o";
                }
                else
                {
                    msg += " ";
                }
            }
        }
    }
}
