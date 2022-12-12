using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class LBSGraphElement : GraphElement
{

    private LBSGraphView rootView;

    public LBSGraphView Root => rootView;

    public LBSGraphElement()
    {
        LoadVisual();
    }

    public LBSGraphElement(LBSGraphView rootView)
    {
        this.rootView = rootView;
        LoadVisual();
    }

    public abstract void LoadVisual();

    public abstract void OnDelete();
}
