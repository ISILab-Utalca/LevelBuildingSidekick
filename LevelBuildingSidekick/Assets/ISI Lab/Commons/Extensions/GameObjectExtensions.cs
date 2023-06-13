using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static void SetParent(this GameObject gameObjct, GameObject other)
    {
        gameObjct.transform.parent = other.transform;
    }
}
