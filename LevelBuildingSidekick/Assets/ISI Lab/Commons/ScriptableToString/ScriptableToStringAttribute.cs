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

    public ScriptableToStringAttribute(Type type)
    {
        //SOs = Resources.FindObjectsOfTypeAll(type).Select(o => o as ScriptableObject).ToList();
        SOs = DirectoryTools.GetScriptables(type);
    }
}
