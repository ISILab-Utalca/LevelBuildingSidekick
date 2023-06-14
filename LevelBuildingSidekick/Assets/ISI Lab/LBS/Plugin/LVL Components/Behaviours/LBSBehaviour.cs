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
    [JsonIgnore]
    protected LBSLayer Owner; // no se si esto tenggo que inicializarlo con un Init() o en el constructor (?)

    public LBSBehaviour(){ }

    public void InternalInit(LBSLayer layer)
    {
        Owner = layer;
    }
    public abstract void Init(LBSLayer layer);

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


}

[System.Serializable]
[RequieredModule(typeof(Exterior))]
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

[System.Serializable]
[RequieredModule(typeof(LBSRoomGraph),typeof(LBSSchema))]
public class SchemaBehaviour : LBSBehaviour
{
    public bool ShowNode = true;
    public string label = "Aqui deberia ir una paleta de habitaciones.";

    public override object Clone()
    {
        return new SchemaBehaviour();
    }

    public override void Init(LBSLayer layer)
    {
       // throw new NotImplementedException();
    }
}




