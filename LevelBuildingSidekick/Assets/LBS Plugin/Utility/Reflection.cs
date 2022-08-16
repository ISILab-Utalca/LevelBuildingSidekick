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
}
