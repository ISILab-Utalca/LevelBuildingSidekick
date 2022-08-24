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

        public static void PrintDerivedTypes(Type baseType)
        {
            var types = FindDerivedTypes(baseType).ToList();
            var msg = "";
            types.ForEach(t => msg += t.Name + "\n");
            Debug.Log(msg);
        }

        public static List<Tuple<T, MethodInfo>> CollectMetohdsByAttribute<T>() where T : Attribute
        {
            var methods = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .Where(m => m.GetCustomAttributes<T>().Count() > 0)
                        .ToArray();

            var toReturn = new List<Tuple<T, MethodInfo>>();
            methods.ToList().ForEach(m => toReturn.Add(new Tuple<T, MethodInfo>(m.GetCustomAttribute<T>(), m)));

            return toReturn;
        }
    }
}

