using LBS.Bundles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[LBSCharacteristic("Connection group","")]
public class LBSDirectionedGroup : LBSCharacteristic, ICloneable
{
    #region SUB-STRUCTURE
    [System.Serializable]
    public class WeigthStruct
    {
        [SerializeField]
        public Bundle target;

        [Range(0f, 1f)]
        public float weigth;
    };
    #endregion
    // falta ver cuando se elmine un hijo sacara de esta lista interna

    [SerializeField]
    public List<WeigthStruct> Weights = new List<WeigthStruct>();

    [JsonIgnore,System.NonSerialized]
    private List<Tuple<Bundle, LBSDirection>> connections = new List<Tuple<Bundle, LBSDirection>>();

    [JsonIgnore]
    public List<LBSDirection> Connections => connections.Select( t=> t.Item2).ToList();

    public LBSDirectionedGroup()
    {

    }

    public override void OnEnable()
    {
        Owner.OnAddChild += OnAddAssetToOwner;
    }

    private void OnAddAssetToOwner(Bundle child)
    {
        var c = new LBSDirection();
        child.AddCharacteristic(c);
        AddTilesChild(child,c);
    }

    public void AddTilesChild(Bundle bundle, LBSDirection connection)
    {
        connections.Add(new Tuple<Bundle, LBSDirection>(bundle, connection));
    }

    public override object Clone()
    {
        var childs = Owner.ChildsBundles;
        childs.ForEach( b => 
        {
            var c = b.GetCharacteristics<LBSDirection>();
            AddTilesChild(b, c[0]);
        });
        return new LBSDirectionedGroup();
    }

    public List<LBSDirection> GetDirs()
    {
        var r = new List<LBSDirection>();
        foreach (var w in Weights)
        {
            r.Add(w.target.GetCharacteristics<LBSDirection>()[0]);
        }
        return r;
    }

    public override bool Equals(object obj)
    {
        return false; // Implentar bien (!!)
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
