using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LBSAttribute : System.Attribute
{

}

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
