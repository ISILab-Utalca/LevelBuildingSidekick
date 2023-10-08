using LBS.Components;
using LBS.Settings;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ConnectedZonesModule : LBSModule
{
    #region FIELDS
    [SerializeField, JsonRequired, SerializeReference]
    List<ZoneEdge> edges = new List<ZoneEdge>();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<ZoneEdge> Edges => new List<ZoneEdge>(edges);
    #endregion

    #region EVENTS
    public event Action<ConnectedZonesModule, ZoneEdge> OnAddEdge;
    public event Action<ConnectedZonesModule, ZoneEdge> OnRemoveEdge;
    #endregion

    #region CONSTRUCTORS
    public ConnectedZonesModule() 
    { 

    }
    #endregion

    #region METHODS
    public void AddEdge(ZoneEdge edge)
    {
        edges.Add(edge);
        OnAddEdge?.Invoke(this, edge);
    }

    public void AddEdge(Zone first, Zone second)
    {
        var edge = new ZoneEdge(first, second);
        edges.Add(edge);
        OnAddEdge?.Invoke(this, edge);
    }

    public ZoneEdge GetEdge(Zone first, Zone second, bool bothDir = true)
    {
        foreach (var edge in edges)
        {
            if(edge.First.Equals(first) && edge.Second.Equals(second))
                return edge;

            if( bothDir && edge.Second.Equals(first) && edge.First.Equals(first))
                return edge;
        }
        return null;
    }

    //[Obsolete("¿Deberia el 'modelo' hacer cosas relativas a distanias?")]
    public ZoneEdge GetEdge(Vector2 position, float delta)
    {
        foreach (var e in edges)
        {
            //var first = new Vector2(e.First.Pivot.x, - e.First.Pivot.y + 1) * Owner.TileSize * LBSSettings.Instance.general.TileSize;
            //var second = new Vector2(e.Second.Pivot.x, - e.Second.Pivot.y + 1) * Owner.TileSize * LBSSettings.Instance.general.TileSize;

            Debug.Log(e.First.Pivot + " - " + e.Second.Pivot + " - " + position);

            var dist = position.DistanceToLine(e.First.Pivot, e.Second.Pivot);
            if (dist < delta)
                return e;
        }
        return null;
    }

    public void RemoveEdge(Zone first, Zone second)
    {
        var edge = GetEdge(first, second);
        if (edge != null)
        {
            edges.Remove(edge);
            OnRemoveEdge?.Invoke(this, edge);
        }
    }

    public void RemoveEdge(ZoneEdge edge)
    {
        edges.Remove(edge);
        OnRemoveEdge?.Invoke(this,edge);
    }

    public void RemoveEdges(Zone zone)
    {
        var toRemove = edges.Where(e => e.First.Equals(zone) || e.Second.Equals(zone)).ToList();
        for(int i = 0; i < toRemove.Count; i++)
        {
            edges.Remove(toRemove[i]);
        }
    }

    public override void Clear()
    {
        edges.Clear();
    }

    public override object Clone()
    {
        var clone = new ConnectedZonesModule();

        var edgesClone = edges.Select(e => e.Clone()).Cast<ZoneEdge>();
        foreach (var edge in edgesClone)
        {
            clone.AddEdge(edge);
        }

        return clone;
    }

    public override Rect GetBounds()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsEmpty()
    {
        return edges.Count <= 0;
    }

    public override void Print()
    {
        string msg = "";
        msg += "Type: " + GetType() + "\n";
        msg += "Hash code: " + GetHashCode() + "\n";
        msg += "ID: " + ID + "\n";
        msg += "\n";
        foreach (var edge in edges)
        {
            msg += edge.First.ID +" - "+ edge.Second.ID + "\n";
        }
        Debug.Log(msg);
    }

    public override void Rewrite(LBSModule module)
    {
        throw new System.NotImplementedException();
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        var other = obj as ConnectedZonesModule;

        if (other == null)  return false;

        var eCount = other.edges.Count;

        if (eCount != this.edges.Count) return false;

        for (int i = 0; i < eCount; i++)
        {
            var e1 = this.edges[i];
            var e2 = other.edges[i];

            if (!e1.Equals(e2)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}

[System.Serializable]
public class ZoneEdge : ICloneable
{
    #region FIELDS
    [SerializeField, SerializeReference, JsonRequired]
    private Zone first;

    [SerializeField, SerializeReference, JsonRequired]
    private Zone second;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public Zone First
    {
        get => first;
        set => first = value;
    }

    [JsonIgnore]
    public Zone Second
    {
        get => second;
        set => second = value;
    }
    #endregion

    #region CONSTRUCTORS
    public ZoneEdge(Zone first, Zone second)
    {
        this.first = first;
        this.second = second;
    }
    #endregion

    #region METHODS
    public object Clone()
    {
        return new ZoneEdge(CloneRefs.Get(first) as Zone, CloneRefs.Get(second) as Zone);
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        var other = obj as ZoneEdge;

        if (other == null) return false;

        if(!this.first.Equals(other.first)) return false;

        if(!this.second.Equals(other.second)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}