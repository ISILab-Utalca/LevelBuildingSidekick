using Commons.Optimization;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    LBSLayer layer;

    VisualElement container;

    /*public AIPanel(Commons.Optimization.BaseOptimizerMetahuristic<IEvaluable> _base)
    {
        this._base = _base;
    }*/

    public AIPanel()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);

        container = this.Q<VisualElement>(name: "Container");
    }

    public void Init(LBSLayer layer)
    {
        Debug.Log("Hya");

        container.Clear();

        this.layer = layer;

        var assist = layer.Assitant;

        for(int i = 0; i < assist.AgentsCount; i++)
        { 
            container.Add(new AIAgentPanel(assist.GetAgent(i)));
        }
    }

}
