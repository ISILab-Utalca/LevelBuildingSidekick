using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class LBSManipulator : MouseManipulator, IManipulatorLBS
{
    public Action OnManipulationStart;
    public Action OnManipulationUpdate;
    public Action OnManipulationEnd;

    public LBSManipulator() { }

    public abstract void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);

    public void AddManipulationStart(Action action)
    {
        OnManipulationStart += action;
    }

    public void AddManipulationUpdate(Action action)
    {
        OnManipulationUpdate += action;
    }

    public void AddManipulationEnd(Action action)
    {
        OnManipulationEnd += action;
    }

    public void RemoveManipulationStart(Action action)
    {
        OnManipulationStart -= action;
    }

    public void RemoveManipulationUpdate(Action action)
    {
        OnManipulationUpdate -= action;
    }

    public void RemoveManipulationEnd(Action action)
    {
        OnManipulationEnd -= action;
    }
}


public interface IManipulatorLBS
{
    public void AddManipulationStart(Action action);
    public void AddManipulationUpdate(Action action);
    public void AddManipulationEnd(Action action);

    public void RemoveManipulationStart(Action action);
    public void RemoveManipulationUpdate(Action action);
    public void RemoveManipulationEnd(Action action);

    public void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);
}