using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drawer
{
    public string modeID;

    public void InternalDraw(ref LBSLayer layer, MainView view)
    {
        view.ClearView();
        Draw(ref layer, view);
    }

    protected abstract void Draw(ref LBSLayer layer, MainView view);
}
