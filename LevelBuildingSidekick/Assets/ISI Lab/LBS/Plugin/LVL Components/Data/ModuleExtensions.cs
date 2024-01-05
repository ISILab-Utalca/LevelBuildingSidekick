using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;

public static class ModuleExtensions
{
    public static T GetModule<T>(this List<LBSModule> modules , string ID = "") where T : LBSModule
    {
        var t = typeof(T);
        foreach (var module in modules)
        {
            if (module is T || Utility.Reflection.IsSubclassOfRawGeneric(t, module.GetType()))
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
    private static Dictionary<int, Dictionary<object,object>> directorys = new Dictionary<int, Dictionary<object, object>>();

    public static void Start()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        
        var c = directorys.ContainsKey(threadId);

        if(c)
        {
            throw new Exception("No puedes iniciar un ciclo de clona teneindo uno previamente iniciado," + threadId);
        }
        else
        {
            lock (directorys)
            {
                directorys[threadId] = new Dictionary<object, object>();
            }
        }
    }

    public static void End()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;

        lock (directorys)
        {
            directorys.Remove(threadId);
        }
    }

    public static void Add(object original, object clone)
    {
        /*
        int threadId = Thread.CurrentThread.ManagedThreadId;
        var x = cicloDeClonados.ContainsKey(threadId);

        if (!x)
        {
            throw new Exception("No puedes añadir si no has iniciado un ciclo de clonado.");
        }

        lock (directorys)
        {
            directorys[threadId][original] = clone;
        }
        */
        //dictionary[original] = clone;
    }


    public static object Get(object original)
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;
        var c = directorys.ContainsKey(threadId);

        if (!c)
        {
            throw new Exception("No puedes obtener si no has iniciado un ciclo de clonado.");
        }

        directorys.TryGetValue(threadId, out var dir);

        /*
        if(dir == null)
        {
            lock (directorys)
            {
                dir = new Dictionary<object, object>();
                directorys[threadId] = dir;
            }
        }
        */

        if (dir.ContainsKey(original))
        {
            return dir[original];
        }
        else
        {
            var clone = (original as ICloneable).Clone();

            directorys.TryGetValue(threadId, out var dir2);

            lock (directorys)
            {
               dir2[original] = clone;
            }
            
            return clone;
        }


        /*
        if(dictionary.ContainsKey(original))
        {
            return dictionary[original];
        }
        else
        {
            var clone = (original as ICloneable).Clone();
            dictionary[original] = clone;
            return clone;
        }
        */
    }

}