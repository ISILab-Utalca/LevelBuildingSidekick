using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class ScriptableObjectReferenceAttribute : PropertyAttribute
{
    public List<ScriptableObject> SOs = new List<ScriptableObject>();
    public Type type;

    public ScriptableObjectReferenceAttribute(Type type, string blackBoard = "")
    {
        this.type = type;

        if (blackBoard == "")
        {
            if (SOs == null || SOs.Count <= 0)
                SOs = LBSAssetsStorage.Instance.Get(type);
        }
        else
        {
            var xx = LBSAssetsStorage.Instance.Get(typeof(LBSIdentifierBundle));
            var x = xx.Find(b => b.name.Equals(blackBoard)) as LBSIdentifierBundle;
            SOs = x.Tags.Cast<ScriptableObject>().ToList();
        }
    }
}
