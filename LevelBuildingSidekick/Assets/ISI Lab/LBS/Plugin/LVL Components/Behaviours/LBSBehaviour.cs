using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public abstract class LBSBehaviour : ICloneable
{
    #region FIELDS
    [NonSerialized, HideInInspector, JsonIgnore]
    protected LBSLayer owner;
    [SerializeField]
    public Texture2D icon;
    [SerializeField]
    public string name;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public LBSLayer Owner
    {
        get => owner;
    }
    #endregion

    #region EVENTS
    
    #endregion

    #region CONSTRUCTORS
    public LBSBehaviour()
    {

    }

    public LBSBehaviour(Texture2D icon, string name)
    {
        this.icon = icon;
        this.name = name;
    }
    #endregion

    #region METHODS
    public virtual void Init(LBSLayer layer)
    {
        owner = layer;
    }

    public abstract object Clone();

    public List<Type> GetRequieredModules ()
    {
        var toR = new List<Type>();
        Type tipo = this.GetType();

        object[] atts = tipo.GetCustomAttributes(true);

        foreach (var att in atts)
        {
            if (att is RequieredModuleAttribute)
            {
                toR.AddRange((att as RequieredModuleAttribute).types);
            }
        }
        return toR;
    }
    #endregion

}

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class SimpleConectedBehaviour : LBSBehaviour
{
    public string label = "Aqui deberia ir una paleta de tipo de conecciones.";

    public override object Clone()
    {
        return new SimpleConectedBehaviour();
    }

    public override void Init(LBSLayer layer)
    {
       // throw new NotImplementedException();
    }
}






