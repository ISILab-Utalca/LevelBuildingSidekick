using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MetadataAttribute : Attribute
{
    public Metadata metadata;

    public MetadataAttribute(string name,string texturePath, string descrition = "")
    {
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        metadata = new Metadata()
        {
            name = name,
            icon = texture,
            description = descrition,
        };
    }
}
