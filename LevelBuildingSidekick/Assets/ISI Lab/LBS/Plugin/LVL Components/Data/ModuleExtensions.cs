using ISILab.Commons.Utility;
using ISILab.LBS.Modules;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    public static class ModuleExtensions
    {
        public static T GetModule<T>(this List<LBSModule> modules, string ID = "") where T : LBSModule
        {
            var t = typeof(T);
            foreach (var module in modules)
            {
                if (module is T || Reflection.IsSubclassOfRawGeneric(t, module.GetType()))
                {
                    if (ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module as T;
                    }
                }
            }
            return null;
        }

        public static List<LBSModule> Clone(this List<LBSModule> modules)
        {
            CloneRefs.Start();
            var clone = modules.Select(m => m.Clone() as LBSModule).ToList();
            CloneRefs.End();

            return clone;
        }
    }

    public static class CloneRefs
    {
        private static Dictionary<int, Dictionary<object, object>> dictionary = new Dictionary<int, Dictionary<object, object>>();

        public static void Start()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            var c = dictionary.ContainsKey(threadId);

            if (c)
            {
                throw new Exception("No puedes iniciar un ciclo de clona teneindo uno previamente iniciado," + threadId);
            }
            else
            {
                lock (dictionary)
                {
                    dictionary[threadId] = new Dictionary<object, object>();
                }
            }
        }

        public static void End()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            lock (dictionary)
            {
                dictionary.Remove(threadId);
            }
        }

        public static object Get(object original)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            var c = dictionary.ContainsKey(threadId);

            if (!c)
            {
                throw new Exception("No puedes obtener si no has iniciado un ciclo de clonado.");
            }

            dictionary.TryGetValue(threadId, out var dir);

            if (dir.ContainsKey(original))
            {
                return dir[original];
            }
            else
            {
                var clone = (original as ICloneable).Clone();

                lock (dictionary)
                {
                    dir[original] = clone;
                }

                return clone;
            }
        }
    }
}