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

        public static ScriptableObject MakeGenericScriptable(object obj)
        {
            return MakeGenericWraper<ScriptableObject>(obj);
        }

        private static T MakeGenericWraper<T>(object obj)
        {
            AssemblyName an = new AssemblyName(obj.ToString() + "_generic");
            var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(an.Name);

            var tb = mb.DefineType(obj.ToString() + "_generic", TypeAttributes.Public, typeof(T));
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

            var ret = Activator.CreateInstance(t);
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
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        public static IEnumerable<Type> GetAllImplementationsOf(Type baseType)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => t.GetInterface(baseType.Name) != null);
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

    }
}

