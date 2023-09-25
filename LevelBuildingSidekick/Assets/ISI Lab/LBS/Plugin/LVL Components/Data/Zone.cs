using LBS.Bundles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Zone : ICloneable
{
    #region FIELDS

    [SerializeField, JsonRequired]
    protected string id = "Zone";
    [SerializeField, JsonRequired, JsonConverter(typeof(Color))]
    protected Color color;
    [SerializeField, JsonRequired, JsonConverter(typeof(Vector2))]
    protected Vector2 pivot;

    //[ScriptableObjectReference(typeof(LBSIdentifier), "Interior Styles")]
    [SerializeField, JsonRequired]
    private List<string> tagsBundles = new List<string>();
    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public string ID
    {
        get => id;
        set => id = value;
    }

    [JsonIgnore]
    public Color Color
    {
        get => color;
        set => color = value;
    }

    [JsonIgnore]
    public Vector2 Pivot
    {
        get => pivot;
        set => pivot = value;
    }

    [JsonIgnore]
    public List<string> TagsBundles
    {
        get => tagsBundles;
        set => tagsBundles = value;
    }
    #endregion

    #region CONSTRUCTORS

    public Zone() { }

    public Zone(string id, Color color)
    {
        this.id = id;
        this.color = color;
    }

    #endregion

    #region METHODS
    public object Clone()
    {
        Zone clone = CloneRefs.Get(this) as Zone;

        if (clone == null)
        {
            clone = new Zone(this.id, this.color);
            CloneRefs.Add(this, clone);
        }

        return clone;
    }

    public override bool Equals(object obj)
    {
        var other = obj as Zone;

        if (other == null) return false;

        if(!id.Equals(other.id)) return false;

        if(!color.Equals(other.color)) return false;

        if(!pivot.Equals(other.pivot)) return false;

        return true;  
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    #endregion
}

public static class ZoneExtension
{
    public static List<Bundle> GetBundles(this Zone zone)
    {
        var bundles = new List<Bundle>();
        var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().Where(b => !b.IsPresset).ToList();
        foreach (var tags in zone.TagsBundles)
        {
            bundles.Add(allBundles.Find(b => b.name.Equals(tags)));
        }
        return bundles;
    }
}