using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class LBSManipulator : MouseManipulator
{
    // ref view
    protected MainView view;

    public Action OnManipulationStart;
    public Action OnManipulationUpdate;
    public Action OnManipulationEnd;

    public LBSManipulator() { }

    public void InitView(ref MainView view)
    {
        this.view = view;

    }

    public abstract void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);


}
