using Commons.Optimization;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class AIPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<AIPanel, VisualElement.UxmlTraits> { }

    public LBSLevelData data;

    public BaseOptimizerMetahuristic<IEvaluable> _base;

    public AIPanel(Commons.Optimization.BaseOptimizerMetahuristic<IEvaluable> _base)
    {
        this._base = _base;
    }

    public AIPanel() { }

    public AIPanel(LBSLevelData levelData)
    {
        this.data = levelData;


        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("AIPanel"); // Editor
        visualTree.CloneTree(this);


        var addLayerBtn = this.Q<Button>("OptimizerB");
        addLayerBtn.clicked += AddLayer;
    }
    private void AddLayer()
    {
        var x = data.Layers;

        UnityEngine.Debug.Log("working");
       
        

    }
}
