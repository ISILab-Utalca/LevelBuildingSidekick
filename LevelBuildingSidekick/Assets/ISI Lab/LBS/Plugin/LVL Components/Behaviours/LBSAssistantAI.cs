using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LBSAssistantAI
{
    [JsonIgnore]
    protected LBSLayer Owner;

    public LBSAssistantAI()
    {

    }

    public List<Type> GetRequieredModules()
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
public class AssistantWFC : LBSAssistantAI
{

}

[System.Serializable]
[RequieredModule(typeof(LBSRoomGraph), typeof(LBSSchema))]
public class AssistantHillClimbing : LBSAssistantAI
{

}
