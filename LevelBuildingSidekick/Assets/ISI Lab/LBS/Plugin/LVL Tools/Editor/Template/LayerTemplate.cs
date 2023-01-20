using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName ="ISILab/LBS/Layer Template")]
public class LayerTemplate : ScriptableObject
{
    public LBSLayer layer;
    public List<LBSMode> modes = new List<LBSMode>();

}

[System.Serializable]
public class LBSMode
{
    public string name;
    public List<LBSTool> toolkit = new List<LBSTool>();
}
