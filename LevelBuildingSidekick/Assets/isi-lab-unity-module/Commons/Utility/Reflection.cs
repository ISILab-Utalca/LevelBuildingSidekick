using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;

namespace ISILab.Commons.Utility
{
    public static class Reflection
    {
        /// <summary>
        /// Get type by name from all loaded assemblies in the current domain.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetType(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Reverse().ToList();
            foreach (var assembly in assemblies)
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all classes with a specific attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Tuple<Type, IEnumerable<T>>> GetClassesWith<T>() where T : Attribute
        {
            var toR = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                let attributes = type.GetCustomAttributes(typeof(T), true)
                where attributes != null && attributes.Length > 0
                select new Tuple<Type, IEnumerable<T>>(type, attributes.Cast<T>());

            return toR.ToList();
        }

        /// <summary>
        /// Get all classes with a specific attribute.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Tuple<Type, IEnumerable<Attribute>>> GetClassesWith(Type type)
        {
            var toR = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                      from _type in assembly.GetTypes()
                      let attributes = _type.GetCustomAttributes(type, true)
                      where attributes != null && attributes.Length > 0
                      select new Tuple<Type, IEnumerable<Attribute>>(_type, attributes as Attribute[]);

            return toR.ToList();
        }

        /// <summary>
        /// Make a generic scriptable object from an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ScriptableObject MakeGenericScriptable(object obj)
        {
            return MakeGenericWraper<ScriptableObject>(obj);
        }

        /// <summary>
        /// Make a generic wrapper from an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static T MakeGenericWraper<T>(object obj) where T : ScriptableObject
        {
            AssemblyName an = new AssemblyName(obj.ToString() + "Generic");
            var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(an.Name);

            var tb = mb.DefineType(obj.ToString() + "Generic", TypeAttributes.Public, typeof(T));
            tb.DefineField("data", obj.GetType(), FieldAttributes.Public);
            Type[] parameterTypes = { obj.GetType() };
            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);
            var defCtor = tb.DefineDefaultConstructor(MethodAttributes.Public);
            ILGenerator ctor1IL = ctor.GetILGenerator();

            var il_gen = ctor.GetILGenerator();
            il_gen.Emit(OpCodes.Ldarg_0);
            il_gen.Emit(OpCodes.Call, typeof(T));
            il_gen.Emit(OpCodes.Ret);
            Type t = tb.CreateType();

            var r = ScriptableObject.CreateInstance(t);
            var pro = t.GetField("data");
            pro.SetValue(r, obj);

            return (T)r;
        }

        /// <summary>
        /// Get all sub classes of a type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllSubClassOf<T>()
        {
            return GetAllSubClassOf(typeof(T));
        }

        /// <summary>
        /// Get all sub classes of a type.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllSubClassOf(Type baseType)
        {
            var toR = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(baseType)));
            return toR;
        }

        /// <summary>
        /// Get all implementations of a type.
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllImplementationsOf(Type baseType)
        {
            var toR = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterface(baseType.Name) != null));
            return toR;
        }

        /// <summary>
        /// Get all implementations of a type.
        /// </summary>
        /// <param name="baseType"></param>
        public static void PrintDerivedTypes(Type baseType)
        {
            var types = GetAllSubClassOf(baseType).ToList();
            var msg = "";
            types.ForEach(t => msg += t.Name + "\n");
            Debug.Log(msg);
        }

        /// <summary>
        /// Collect all methods with a specific attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        /// <summary>
        /// Collect all methods with a specific attribute.
        /// </summary>
        /// <param name="generic"></param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic.Equals(cur)  || generic.Equals(toCheck))
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}

