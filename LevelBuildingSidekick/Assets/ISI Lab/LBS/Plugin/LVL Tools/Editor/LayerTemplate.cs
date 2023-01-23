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
    [SerializeField]
    public string name;
    [SerializeField]
    public string drawer;
    [SerializeField]
    public List<LBSTool> toolkit = new List<LBSTool>();

    [NonSerialized]
    public Drawer _drawer;

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

    public LBSMode(string name, Drawer drawer, List<LBSTool> toolkit)
    {
        this.name = name;
        this.Drawer = drawer;
        this.toolkit = toolkit;
    }

}
