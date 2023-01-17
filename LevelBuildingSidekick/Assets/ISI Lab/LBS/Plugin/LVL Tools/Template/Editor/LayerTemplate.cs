using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName ="ISILab/LBS/Layer Template")]
public class LayerTemplate : ScriptableObject
{
    [ContextMenuItem("Set as Interior", "InteriorConstruct")]
    [ContextMenuItem("Set as Exterior", "ExteriorConstruct")]
    [ContextMenuItem("Set as Population", "PopulationConstruct")]
    //[ContextMenuItem("Set as Quest", "InteriorConstruct")]
    public LBSLayer layer;

    public List<Mode> modes;

    [System.Serializable]
    public class Mode
    {
        public string name;
        public ToolkitTemplate tools;
    }
}


