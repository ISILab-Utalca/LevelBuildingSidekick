using UnityEngine;

/*
DisplayNameAttribute.cs
DisplayNameAttribute (c) Nine Penguins (Samantha Stahlke) 2019

PathOSDisplayNameAttribute.cs
PathOSDisplayNameAttribute (c) Gabriel Balassa 2024
*/


public class PathOSDisplayNameAttribute : PropertyAttribute
{
    public string displayName { get; private set; }

    public PathOSDisplayNameAttribute(string displayName)
    {
        this.displayName = displayName;
    }
}
