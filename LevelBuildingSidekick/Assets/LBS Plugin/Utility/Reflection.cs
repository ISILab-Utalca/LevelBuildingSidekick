using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

namespace Utility
{
    public static class Reflection
    {
        public static IEnumerable<Type> FindDerivedTypes(Type baseType)
        {
            //var assembly = baseType.Assembly;
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }
    }
}

