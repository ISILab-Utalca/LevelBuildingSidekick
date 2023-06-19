using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class LBSAssistantAI: ICloneable 
{
    [JsonIgnore]
    protected LBSLayer Owner;

    [JsonIgnore]
    public Action OnStart;
    [JsonIgnore]
    public Action OnTermination;


    public LBSAssistantAI(){ }

    public void InternalInit(LBSLayer layer)
    {
        Owner = layer;
        Init(layer);
    }

    public abstract void Init(LBSLayer layer);

    public abstract void Execute();

    public abstract VisualElement GetInspector(); // tiene que estar aqui (???)

    public abstract object Clone();

    public List<Type> GetRequieredModules()
    {
        var toR = new List<Type>();
        Type type = this.GetType();

        object[] atts = type.GetCustomAttributes(true);

        foreach (var att in atts)
        {
            if (att is RequieredModuleAttribute)
            {
                toR.AddRange((att as RequieredModuleAttribute).types);
            }
        }
        return toR;
    }
    /*
    public Metadata GetMetaData()
    {
        Type type = this.GetType();
        object[] atts = type.GetCustomAttributes(true);

        foreach (var att in atts)
        {
            var attribute = att as MetadataAttribute;
            if(attribute !=null)
            {
                return attribute.metadata;
            }
        }
        return default(Metadata);
    }*/
}

