using System;

[AttributeUsage(AttributeTargets.Class)]
public class ChromosomeFromModuleAttribute : LBSAttribute
{
    public Type type;

    public ChromosomeFromModuleAttribute(Type t)
    {
        type = t;
    }
}