using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LBSCustomEditorAttribute : LBSAttribute
{
    public Type type;
    public string name;

    public LBSCustomEditorAttribute(string name,Type type)
    {
        this.name = name;
        this.type = type;
    }

}

public static class LBS_Editor
{
    public static List<Tuple<Type,IEnumerable<LBSCustomEditorAttribute>>> pairsEditors;

    public static List<Tuple<Type,IEnumerable<DrawerAttribute>>> pairDrawers;

    public static Type GetEditor<T>()
    {
        return GetEditor(typeof(T));
    }

    public static Type GetEditor(Type targetType)
    {
        if (pairsEditors == null)
            pairsEditors = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>();

        foreach (var pair in pairsEditors)
        {
            if (pair.Item2.ToList()[0].type == targetType)
            {
                return pair.Item1;
            }
        }
        return null;

    }

    public static Type GetDrawer<T>()
    {
        return GetDrawer(typeof(T));
    }

    public static Type GetDrawer(Type targetType)
    {
        if(pairDrawers == null)
            pairDrawers = Utility.Reflection.GetClassesWith<DrawerAttribute>();

        foreach (var pair in pairDrawers)
        {
            var t = pair.Item2.ToList()[0].type;
            if (t == targetType)
            {
                return pair.Item1;
            }
        }
        return null;
    }

}