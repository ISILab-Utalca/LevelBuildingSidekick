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
    }

    public override object Clone()
    {
        return new AssistantMapElite();
    }
}


