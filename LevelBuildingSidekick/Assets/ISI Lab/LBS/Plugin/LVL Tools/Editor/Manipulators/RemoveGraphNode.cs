using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveGraphNode<T> : LBSManipulator where T : LBSNode
{
    // ref Data
    private GraphModule<T> module;

    public RemoveGraphNode() : base() { }

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<GraphModule<T>>();
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        OnManipulationStart?.Invoke();

        var node = e.target as LBSNodeView<T>;
        
        if (node == null)
        {
            return;
        }

        module.RemoveNode(node.Data);
        OnManipulationEnd?.Invoke();
    }
}
