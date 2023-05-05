using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class LBSDirectionedGroup : LBSCharacteristic, ICloneable
{
    #region SUB-STRUCTURE
    [System.Serializable]
    public struct weigthStruct
    {
        public GameObject target;

        [Range(0f, 1f)]
        public float weigth;
    };
    #endregion
    // falta ver cuando se elmine un hijo sacara de esta lista interna

    [JsonIgnore]
    private List<Tuple<Bundle, LBSDirection>> connections = new List<Tuple<Bundle, LBSDirection>>();

    [JsonIgnore]
    public List<LBSDirection> Connections => connections.Select( t=> t.Item2).ToList();

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
        Owner.ChildsBundles.ForEach( b => AddTilesChild(b, b.GetCharacteristic<LBSDirection>()));
        return new LBSDirectionedGroup();
    }

   
}
