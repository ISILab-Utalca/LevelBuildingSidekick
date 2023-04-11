using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class ModuleTexturizerAttribute : LBSAttribute
{
    public Type type;
    public ModuleTexturizerAttribute(Type type)
    {
        this.type = type;
    }
}
