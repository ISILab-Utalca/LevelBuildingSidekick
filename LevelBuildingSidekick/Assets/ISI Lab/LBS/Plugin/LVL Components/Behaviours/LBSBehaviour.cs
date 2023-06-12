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
public abstract class LBSBehaviour
{
    [JsonIgnore]
    protected LBSLayer Owner; // no se si esto tenggo que inicializarlo con un Init() o en el constructor (?)

    public LBSBehaviour()
    {

    }

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

}

[System.Serializable]
[RequieredModule(typeof(LBSRoomGraph),typeof(LBSSchema))]
public class SchemaBehaviour : LBSBehaviour
{
    public string TEST2 = "Adios mundo :C";
}




