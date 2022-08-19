using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Reflection
{
    public static IEnumerable<Type> FindDerivedTypes(Type baseType)
    {
        var assembly = baseType.Assembly;
        return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
    }

    public static void PrintDerivedTypes(Type baseType)
    {
        var types = FindDerivedTypes(baseType).ToList();
        var msg = "";
        types.ForEach(t => msg+= t.Name +"\n");
        Debug.Log(msg);
    }
}
