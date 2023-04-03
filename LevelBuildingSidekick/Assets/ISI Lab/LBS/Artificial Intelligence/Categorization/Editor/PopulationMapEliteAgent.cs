using LBS.AI;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class PopulationMapEliteAgent : LBSAIAgent
{
    MapEliteWindow mapElites;
     

    public PopulationMapEliteAgent(LBSLayer layer, string id) : base(layer, id, "PopulationMapEliteAgent")
    {

    }

    public override void Execute()
    {
        Init(ref layer);
    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(ref LBSLayer layer)
    {
        mapElites = MapEliteWindow.GetWindow<MapEliteWindow>();
        mapElites.SetLayer(layer);
    }

    public override object Clone()
    {
        return new PopulationMapEliteAgent(layer, id);
    }
}
