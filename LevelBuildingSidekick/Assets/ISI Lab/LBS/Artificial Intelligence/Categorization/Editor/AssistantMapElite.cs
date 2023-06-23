using LBS.AI;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using LBS.Components.TileMap;
using System.Linq;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Populations;
using Commons.Optimization.Evaluator;
using Commons.Optimization.Terminations;
using LBS.Components.Graph;
using LBS.Tools.Transformer;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


[System.Serializable]
//[RequieredModule(typeof(TaggedTileMap))]
public class AssistantMapElite : LBSAssistantAI
{

    public string AAAA = "AAAAA";

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


