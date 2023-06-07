using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public class ScriptableObjectReferenceAttribute : PropertyAttribute
{
    public List<ScriptableObject> SOs = new List<ScriptableObject>();
    public Type type;

    public ScriptableObjectReferenceAttribute(Type type)
    {
        this.type = type;
        if (SOs == null || SOs.Count <= 0)
            SOs = DirectoryTools.GetScriptables(type);
    }
}
