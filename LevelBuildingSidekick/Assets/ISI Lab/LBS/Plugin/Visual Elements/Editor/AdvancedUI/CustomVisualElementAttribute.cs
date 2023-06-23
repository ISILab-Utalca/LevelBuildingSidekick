using System;

[AttributeUsage(AttributeTargets.Class)]
public class CustomVisualElementAttribute : LBSAttribute
{
    public Type type;

    public CustomVisualElementAttribute(Type t)
    {
        type = t;
    }
}
