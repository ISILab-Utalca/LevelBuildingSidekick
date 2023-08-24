using LBS.Assisstants;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[RequieredModule(typeof(ExteriorModule))]
public class AssitantWFC : LBSAssistant
{
    public AssitantWFC(Texture2D icon, string name) : base(icon, name)
    {
    }

    public override object Clone()
    {
        return new AssitantWFC(this.Icon,this.Name);
    }

    public override void Execute()
    {
        Debug.Log("No implementado WFC 'Execute()'");
    }

    public override void OnAdd(LBSLayer layer)
    {
        Debug.Log("No implementado WFC 'Init(layer)'");
    }
}
