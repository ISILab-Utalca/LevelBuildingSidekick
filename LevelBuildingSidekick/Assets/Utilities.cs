using LevelBuildingSidekick.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static void Print(this SchemaData schema)
    {
        string[,] m = schema.GetMatrix();
    }
}
