using System;
using System.Collections;
using System.Collections.Generic;
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
