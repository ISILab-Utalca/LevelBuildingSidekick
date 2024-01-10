using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class StartQNode : QuestNodeView
{
    private static VisualTreeAsset view;

    public StartQNode()
    {
        if (view == null)
        {
            view = DirectoryTools.SearchAssetByName<VisualTreeAsset>("StartQNode");
        }
        view.CloneTree(this);
    }
}
