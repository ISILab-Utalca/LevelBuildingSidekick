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
    [JsonRequired, SerializeReference]
    public List<LBSMode> modes = new List<LBSMode>();
    [JsonRequired, SerializeReference]
    public List<Transformer> transformers = new List<Transformer>();
}

[System.Serializable]
public class LBSMode
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string drawer;
    [SerializeField]
    public string module;
    [SerializeField, SerializeReference]
    public List<LBSTool> toolkit = new List<LBSTool>();

    [NonSerialized]
    public Drawer _drawer;

    [JsonIgnore]
    public Drawer Drawer {
        get
        {
            if(_drawer == null)
            {
                var mType = Type.GetType(this.drawer);
                _drawer = Activator.CreateInstance(mType) as Drawer;
            }
            return _drawer;
        }

        set
        {
            drawer = value.GetType().FullName;
            _drawer = value;
        }
    }

    [JsonIgnore]
    public Type ModuleType
    {
        get
        {
            var x = Type.GetType(module);
            return x;
        }
        set => module = value.FullName;
    }

    public LBSMode(string name,Type module, Drawer drawer, List<LBSTool> toolkit)
    {
        this.name = name;
        this.ModuleType = module;
        this.Drawer = drawer;
        this.toolkit = toolkit;
    }

}
