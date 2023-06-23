using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Wip : LBSInspector
{
    public Wip()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Wip");
        visualTree.CloneTree(this);
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        //throw new System.NotImplementedException();
    }
}
