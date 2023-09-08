using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;

namespace Utility
{
    public static class Reflection
    {
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

        public static List<Tuple<Type, IEnumerable<T>>> GetClassesWith<T>() where T : Attribute
        {
            var toR = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                let attributes = type.GetCustomAttributes(typeof(T), true)
                where attributes != null && attributes.Length > 0
                select new Tuple<Type, IEnumerable<T>>(type, attributes.Cast<T>());

            return toR.ToList();
        }

        public static List<Tuple<Type, IEnumerable<Attribute>>> GetClassesWith(Type type)
        {
            var toR = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                      from _type in assembly.GetTypes()
                      let attributes = _type.GetCustomAttributes(type, true)
                      where attributes != null && attributes.Length > 0
                      select new Tuple<Type, IEnumerable<Attribute>>(_type, attributes as Attribute[]);

            return toR.ToList();
        }

        public static ScriptableObject MakeGenericScriptable(object obj)
        {
            return MakeGenericWraper<ScriptableObject>(obj);
        }

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

            {
                //  Creation b constructor    
                var il_gen = ctor.GetILGenerator();
                il_gen.Emit(OpCodes.Ldarg_0);
                il_gen.Emit(OpCodes.Call, typeof(T));
                il_gen.Emit(OpCodes.Ret);
            }
            Type t = tb.CreateType();

            var ret = ScriptableObject.CreateInstance(t);// Activator.CreateInstance(t);
            var pro = t.GetField("data");
            pro.SetValue(ret, obj);

            return (T)ret;
        }

        public static IEnumerable<Type> GetAllSubClassOf<T>()
        {
            return GetAllSubClassOf(typeof(T));
        }

        public static IEnumerable<Type> GetAllSubClassOf(Type baseType)
        {
            var toR = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(baseType)));
            return toR;
        }

        public static IEnumerable<Type> GetAllImplementationsOf(Type baseType)
        {
            var toR = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetInterface(baseType.Name) != null));
            return toR;
        }

        public static void PrintDerivedTypes(Type baseType)
        {
            var types = GetAllSubClassOf(baseType).ToList();
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

