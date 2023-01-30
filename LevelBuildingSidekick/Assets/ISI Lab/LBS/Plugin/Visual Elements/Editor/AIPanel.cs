using Commons.Optimization;
using LBS.Components;
using LBS.Components.Graph;
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

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    public LBSLevelData data;
    private LBSSchema schema;
    private LBSRoomGraph graph;

    /*public AIPanel(Commons.Optimization.BaseOptimizerMetahuristic<IEvaluable> _base)
    {
        this._base = _base;
    }*/

    public AIPanel() { }

    public AIPanel(LBSLevelData levelData)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);

        this.data = levelData;
        
        var HillClimbingBtn = this.Q<Button>("OptimizerB");
        HillClimbingBtn.clicked += Optimize;
    }
    private void Optimize()
    {
        for (int i = 0; i < data.Layers.Count; i++)
        {
            if (data.Layers[i].ID == "Interior")
            {
                UnityEngine.Debug.Log("Found layer: " + data.Layers[i].Name);
                schema = data.Layers[i].GetModule<LBSSchema>();
                graph = data.Layers[i].GetModule<LBSRoomGraph>();
                UnityEngine.Debug.Log("Nodos en graph: " + graph.NodeCount);       
                break;
            }
        }
    }
}
