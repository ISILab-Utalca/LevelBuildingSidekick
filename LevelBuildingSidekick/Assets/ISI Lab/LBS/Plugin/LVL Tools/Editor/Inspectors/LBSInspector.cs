using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSInspector : VisualElement // estaq clase deberia ser buscada por atributos
{
    public abstract void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module);

    public abstract void OnLayerChange(LBSLayer layer);
}
