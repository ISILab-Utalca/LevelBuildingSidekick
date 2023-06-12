using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetDerivedTypes(this IEnumerable<Type> types, Type baseType)
    {
        return types.Where(t => baseType.IsAssignableFrom(t) && t != baseType);
    }
}