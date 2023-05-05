using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSCustomEditorAttribute : LBSAttribute
{
    public Type type;

    public LBSCustomEditorAttribute(Type type)
    {
        this.type = type;
    }

}
