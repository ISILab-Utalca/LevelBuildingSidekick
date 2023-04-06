using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class LBSAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Method)]
public class LBSWindowAttribute : LBSAttribute
{
    private string name;
    public string Name => name;

    public LBSWindowAttribute(string name)
    {
        this.name = name;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LBSSearchAttribute : LBSAttribute
{
    private string name;
    private string iconPath;

    public string Name => name;
    public Texture2D Icon => null; // implementar icon (!!!)

    public LBSSearchAttribute(string name, string iconPath)
    {
        this.name = name;
        this.iconPath = iconPath;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LBSBehaviourAttribute : LBSSearchAttribute
{
    public LBSBehaviourAttribute(string name, string iconPath) : base(name, iconPath){ }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LBSRuleAttribute : LBSSearchAttribute
{
    public LBSRuleAttribute(string name, string iconPath) : base(name, iconPath) { }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LBSModificadorAttribute : LBSSearchAttribute
{
    public LBSModificadorAttribute(string name, string iconPath) : base(name, iconPath) { }
}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class LBSCharacteristicAttribute : LBSSearchAttribute
{
    public LBSCharacteristicAttribute(string name, string iconPath) : base(name, iconPath) { }
}