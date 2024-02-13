using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSInspector : VisualElement
{
    public abstract void SetTarget(LBSLayer layer);

    public virtual void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour) { } // FIX: This function is not called anywhere, it is not working

    public virtual void Repaint() { }
}
