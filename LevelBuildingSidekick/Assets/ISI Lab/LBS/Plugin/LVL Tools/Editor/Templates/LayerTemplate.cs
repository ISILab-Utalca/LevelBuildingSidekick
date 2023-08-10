using LBS.Components;
using LBS.Components.Graph;
using LBS.Tools.Transformer;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "ISILab/LBS/Layer Template")]
public class LayerTemplate : ScriptableObject
{
    [JsonRequired, SerializeField]
    public LBSLayer layer;

    public void Clear()
    {
        this.layer = new LBSLayer();
    }
}
