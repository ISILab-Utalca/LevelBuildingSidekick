using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public class ScriptableToStringAttribute : PropertyAttribute
{
    public List<ScriptableObject> SOs;
    public Type type;

    public ScriptableToStringAttribute(Type type)
    {
        this.type = type;
        if (SOs == null || SOs.Count <= 0)
            SOs = DirectoryTools.GetScriptables(type);
    }
}
