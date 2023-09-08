using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConstrainsZonesModule : LBSModule
{
    #region FIELDS
    [SerializeField, JsonRequired]
    private List<ConstraintPair> pairs = new();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<ConstraintPair> Constraints => pairs;
    #endregion

    #region CONSTRUCTORS
    public ConstrainsZonesModule() : base()
    {
        this.id = GetType().Name;
    }

    public ConstrainsZonesModule(List<ConstraintPair> pairs)
    {
        this.pairs = pairs;
    }
    #endregion

    #region METHODS
    public Constraint GetLimits(Zone zone)
    {
        foreach (var pair in pairs)
        {
            if(pair.Zone == zone)
            {
                return pair.Constraint;
            }
        }
        return null;
    }

    public void RecalculateConstraint(List<Zone> zones)
    {
        var temp = new List<ConstraintPair>(pairs);
        pairs.Clear();
        foreach (var zone in zones)
        {
            if (temp.Any(t => t.Zone == zone))
            {
                var x = temp.Find(t => t.Zone == zone);
                pairs.Add(x);
                continue;
            }

            var xx = new ConstraintPair(zone, new Constraint(3, 4,3, 4));
            pairs.Add(xx);
        }
    }

    public override void Print()
    {
        throw new System.NotImplementedException();
    }

    public override void Clear()
    {
        pairs.Clear();
    }

    public override bool IsEmpty()
    {
        return pairs.Count <= 0;
    }

    public override object Clone()
    {
        var clone = new ConstrainsZonesModule();
        clone.pairs = this.pairs.Select(c => c.Clone()).Cast<ConstraintPair>().ToList();

        return clone;
    }

    public override Rect GetBounds()
    {
        throw new System.NotImplementedException();
    }

    public override void Reload(LBSLayer layer)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnAttach(LBSLayer layer)
    {

    }

    public override void OnDetach(LBSLayer layer)
    {

    }

    public override void Rewrite(LBSModule module)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}


[System.Serializable]
public class ConstraintPair : ICloneable
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private Zone zone;
    [SerializeField, SerializeReference, JsonRequired]
    private Constraint constraint;
    #endregion

    #region
    [JsonIgnore]
    public Zone Zone => zone;

    [JsonIgnore]
    public Constraint Constraint => constraint;
    #endregion

    public ConstraintPair(Zone zone, Constraint constraint)
    {
        this.zone = zone;
        this.constraint = constraint;
    }

    public object Clone()
    {
        var clone = new ConstraintPair(zone.Clone() as Zone,constraint.Clone() as Constraint);
        return clone;
    }
}

[System.Serializable]
public class Constraint : ICloneable
{
    public float minWidth;
    public float minHeight;
    public float maxWidth;
    public float maxHeight;

    public float WidthMid => (minWidth + maxWidth) / 2f;
    public float HeightMid => (minHeight + maxHeight) / 2f;

    public Constraint(float minWidth, float minHeight, float maxWidth, float maxHeight)
    {
        this.minWidth = minWidth;
        this.minHeight = minHeight;
        this.maxWidth = maxWidth;
        this.maxHeight = maxHeight;
    }

    public object Clone()
    {
        var clone = new Constraint(minWidth, minHeight, maxWidth, maxHeight);
        return clone;
    }
}