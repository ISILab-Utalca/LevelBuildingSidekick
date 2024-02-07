using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TileView : GraphElement
{
    protected LBSTile data;

    public LBSTile Data => data;

    public TileView(LBSTile tile, string uxml = null)
    {
        data = tile;
        if(uxml != null)
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>(uxml);
            visualTree.CloneTree(this);
        }
    }
}
