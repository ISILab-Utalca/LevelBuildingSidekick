using LBS.Behaviours;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSInspector : VisualElement // estaq clase deberia ser buscada por atributos
{
    public abstract void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour);

    public abstract void OnLayerChange(LBSLayer layer);

    public virtual void Repaint() { }
}
