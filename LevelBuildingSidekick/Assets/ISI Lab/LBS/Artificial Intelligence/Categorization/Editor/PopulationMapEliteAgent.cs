using LBS.AI;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulationMapEliteAgent : LBSAIAgent
{
    MapElites mapElites;

    public PopulationMapEliteAgent(LBSLayer layer, string id) : base(layer, id, "PopulationMapEliteAgent")
    {

    }

    public override void Execute()
    {
        var wnd = MapEliteWindow.GetWindow<MapEliteWindow>();
    }

    public override VisualElement GetInspector()
    {
        throw new System.NotImplementedException();
    }

    public override void Init(ref LBSLayer layer)
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        return new PopulationMapEliteAgent(layer, id);
    }
}
