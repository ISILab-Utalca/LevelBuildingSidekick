using LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Drawer
{
    public string modeID;

    public Drawer() { }

    public abstract void Draw(ref LBSLayer layer, MainView view);

    public abstract void Draw(LBSBehaviour behaviour, MainView view);
}
