using Commons.Optimization;
using Commons.Optimization.Terminations;
using GeneticSharp.Domain.Chromosomes;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class AITest : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AITest, VisualElement.UxmlTraits> { }
    List<int> b;
    public LBSLevelData data;
    private AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema;
    private GraphModule<RoomNode> graph;
    Hill2<IEvaluable> HC;

    //listaIeva s;
    //listaIeva x;

    /*public AIPanel(Commons.Optimization.BaseOptimizerMetahuristic<IEvaluable> _base)
    {
        this._base = _base;
    }*/

    public AITest() { }

    public AITest(LBSLevelData levelData)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AITest"); // Editor
        visualTree.CloneTree(this);

        this.data = levelData;
        
        var HillClimbingBtn = this.Q<Button>("OPB");
        HillClimbingBtn.clicked += Optimize;
    }
    private void Optimize()
    {  
        for (int i = 0; i < data.Layers.Count; i++)
        {
            if (data.Layers[i].ID == "Interior")
            {
                UnityEngine.Debug.Log("Found layer: " + data.Layers[i].Name);
                graph = data.Layers[i].GetModule<GraphModule<RoomNode>>();
                //UnityEngine.Debug.Log("Nodos en graph: " + graph.NodeCount);
                //UnityEngine.Debug.Log("Concexciones en graph: " + graph.EdgeCount);
                schema = data.Layers[i].GetModule<AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>>();
                //UnityEngine.Debug.Log("Areas en Schema: " + schema.RoomCount);
                //UnityEngine.Debug.Log("Areas en lista Areas en Schema: " + schema.Areas.Count);
                break;
            }
        }


        listaIeva s = new();
        listaIeva x = new();

        for(int i = 0; i < 15; i++)
            s.a.Add(i);

        for (int i = 0; i < 5; i++)
            x.a.Add(i+2);

        HC = new Hill2<IEvaluable>(schema as IEvaluable, new WeightedEvaluator(), new FitnessStagnationTermination(), graph as IEvaluable);
        HC.Start();       
    }
}
