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
    public static List<Tuple<Type,IEnumerable<LBSCustomEditorAttribute>>> pairs;

    public static Type GetEditor<T>()
    {
        return GetEditor(typeof(T));
    }

    public static Type GetEditor(Type type)
    {
        if (pairs == null)
            pairs = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>();

        foreach (var pair in pairs)
        {
            if (pair.Item2.ToList()[0].type == type)
            {
                return pair.Item1;
            }
        }
        return null;

    }
}