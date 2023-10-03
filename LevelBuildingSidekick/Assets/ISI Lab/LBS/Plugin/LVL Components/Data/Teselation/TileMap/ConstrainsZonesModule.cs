using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConstrainsZonesModule : LBSModule , ISelectable
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private List<ConstraintPair> pairs = new();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<ConstraintPair> Constraints => pairs;
    #endregion

    #region EVENTS
    public event Action<ConstrainsZonesModule, ConstraintPair> OnAddPair;
    public event Action<ConstrainsZonesModule, ConstraintPair> OnRemovePair;
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

    public void AddPair(Zone zone, Vector2 minArea, Vector2 maxArea)
    {
        var pair = new ConstraintPair(zone, new Constraint(minArea.x, minArea.y, maxArea.x, maxArea.y));
        pairs.Add(pair);
        OnAddPair?.Invoke(this, pair);
    }

    public void RemovePair(Zone zone)
    {
        foreach (var pair in pairs)
        {
            if(pair.Zone == zone)
            {
                pairs.Remove(pair);
                OnRemovePair?.Invoke(this, pair);
                return;
            }
        }
    }

    public void RecalculateConstraint(List<Zone> zones) // parche
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

            var minArea = new Vector2(3,3);
            var maxArea = new Vector2(4,4);
            AddPair(zone,minArea,maxArea);
        }
    }

    public override void Print()
    {
        string msg = "";
        msg += "Type: " + GetType() + "\n";
        msg += "Hash code: " + GetHashCode() + "\n";
        msg += "ID: " + ID + "\n";
        msg += "\n";
        foreach (var pair in pairs)
        {
            msg += pair.Zone + "\n";
            var c= pair.Constraint;
            msg += "- (" + c.minWidth + "," + c.minHeight + ")\n";
            msg += "- (" + c.maxWidth + "," + c.maxHeight + ")\n";
        }
        Debug.Log(msg);
    }

    public override void Clear()
    {
        pairs.Clear();
    }

    public override bool IsEmpty()
    {
        return pairs.Count <= 0;
    }

    public override Rect GetBounds()
    {
        throw new System.NotImplementedException();
    }

    public override object Clone()
    {
        var clone = new ConstrainsZonesModule();
        clone.pairs = this.pairs.Select(c => c.Clone()).Cast<ConstraintPair>().ToList();

        return clone;
    }

    public override void Rewrite(LBSModule module)
    {
        throw new System.NotImplementedException();
    }

    public override bool Equals(object obj)
    {
        var other = obj as ConstrainsZonesModule;

        if (other == null) return false;
        
        var pCount = other.pairs.Count;

        if (pCount != this.pairs.Count) return false;

        for (int i = 0; i < pCount; i++)
        {
            var p1 = this.pairs[i];
            var p2 = other.pairs[i];

            if (!p1.Equals(p2)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public List<object> GetSelected(Vector2Int position)
    {
        var tor = new List<object>();
        for (int i = 0; i < pairs.Count; i++)
        {
            var zone = pairs[i].Zone;
            var size = new Vector2(pairs[i].Constraint.maxWidth, pairs[i].Constraint.maxHeight);

            var pos = zone.Pivot - (size / 2f);
            var rect = new Rect(pos, size);
            if (rect.Contains(position))
            {
                tor.Add(pairs[i].Constraint);
            }
        }

        return tor;
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
        var clone = new ConstraintPair(CloneRefs.Get(zone) as Zone, CloneRefs.Get(constraint) as Constraint);
        return clone;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        var other = obj as ConstraintPair;

        if (other == null) return false;

        if(!this.zone.Equals(other.zone)) return false;

        if(!this.constraint.Equals(other.constraint)) return false;

        return true;  
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

[System.Serializable]
public class Constraint : ICloneable
{
    public float minWidth;
    public float minHeight;
    public float maxWidth;
    public float maxHeight;

    [JsonIgnore]
    public float WidthMid => (minWidth + maxWidth) / 2f;
    [JsonIgnore]
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

    // override object.Equals
    public override bool Equals(object obj)
    {
        var other = obj as Constraint;

        if (other == null) return false;

        if(this.minWidth != other.minWidth) return false;
        if(this.minHeight != other.minHeight) return false;
        if(this.maxWidth != other.maxWidth) return false;
        if(this.maxHeight != other.maxHeight) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}