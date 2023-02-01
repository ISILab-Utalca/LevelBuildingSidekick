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

//Clase temporal para testear, eliminar ya que la principal es AIPanel.
public class AITest : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AITest, VisualElement.UxmlTraits> { }
    List<int> b;
    public LBSLevelData data;
    private AreaTileMap<TiledArea> schema;
    private GraphModule<RoomNode> graph;
    Hill2<IEvaluable> HC;

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
                //En graph como en schema el GetModule no esta retornado correctamente los modulos de LBSRoomGraph y
                //LBSSchema, por lo que se tuvo que utilizar a las padres como tipo de datos. (!!)
                graph = data.Layers[i].GetModule<GraphModule<RoomNode>>();
                schema = data.Layers[i].GetModule<AreaTileMap<TiledArea>>();
                break;
            }
        }

        HC = new Hill2<IEvaluable>(schema as IEvaluable, new WeightedEvaluator(), new FitnessStagnationTermination(), graph as IEvaluable);
        HC.Start();       
    }
}
