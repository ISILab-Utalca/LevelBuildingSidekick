using LBS.AI;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
[RequieredModule(typeof(TaggedTileMap))]
public class AssistantMapElite : LBSAssistantAI
{
    MapEliteWindow mapElites;
     
    public AssistantMapElite(){ }

    public override void Execute()
    {
        // esto deveria ejecutarse pero como esta e n una ventana mejor no (!!)
    }

    public override VisualElement GetInspector()
    {
        return new Label("Inspector MapElite");
    }

    public override void Init(LBSLayer layer)
    {
        mapElites = MapEliteWindow.GetWindow<MapEliteWindow>();
        mapElites.SetLayer(layer);
    }

    public override object Clone()
    {
        return new AssistantMapElite();
    }
}

[System.Serializable]
[RequieredModule(typeof(Exterior))]
public class AssitantWFC : LBSAssistantAI
{
    public override object Clone()
    {
        return new AssitantWFC();
    }

    public override void Execute()
    {
        Debug.Log("No implementado WFC 'Execute()'");
    }

    public override VisualElement GetInspector()
    {
        return new Label("No implementado");
    }

    public override void Init(LBSLayer layer)
    {
        Debug.Log("No implementado WFC 'Init(layer)'");
    }
}

[System.Serializable]
//[RequieredModule(typeof(XXX))]
public class AssistentGrammar : LBSAssistantAI
{
    public override object Clone()
    {
        return new AssistentGrammar();
    }

    public override void Execute()
    {
        Debug.Log("No implementado Grammar 'Execute()'");
    }

    public override VisualElement GetInspector()
    {
        return new Label("No implementado");
    }

    public override void Init(LBSLayer layer)
    {
        Debug.Log("No implementado Grammar 'Init(layer)'");
    }
}