using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateGraph<T> : LBSManipulator where T : LBSNode
{
    protected GraphModule<T> module;

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        this.module = layer.GetModule<GraphModule<T>>();
        this.MainView = view;
    }
}
