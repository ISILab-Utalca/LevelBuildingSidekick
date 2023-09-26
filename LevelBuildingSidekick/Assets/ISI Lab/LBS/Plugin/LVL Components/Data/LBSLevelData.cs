using LBS;
using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LBSLevelData
{
    #region FIELDS
    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSLayer> layers = new List<LBSLayer>();

    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSQuest> quests = new List<LBSQuest>();
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<LBSLayer> Layers => layers;

    [JsonIgnore]
    public int LayerCount => layers.Count;
    #endregion

    #region EVENTS
    public event Action<LBSLevelData> OnChanged;
    #endregion

    #region METHODS
    public void Reload()
    {
        foreach (var layer in layers)
        {
            layer.Reload();
            layer.OnAddModule += (layer, module) => this.OnChanged?.Invoke(this);
            layer.Parent = this;
        }
    }

    public LBSLayer GetLayer(int index)
    {
        return layers[index];
    }

    /// <summary>
    /// Retrieves a representation from the list of layers by its ID
    /// and returns it. If the representation is not found or an exception
    /// is thrown, returns null.
    /// </summary>
    /// <param name="id">The ID of the representation to retrieve</param>
    /// <returns>The representation with the specified ID or null</returns>
    public LBSLayer GetLayer(string id)
    {
        try
        {
            return layers.Find(l => l.ID == id);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return null;
    }

    /// <summary>
    /// Add a new representation, if a representation of
    /// the type delivered already exists, it overwrites it.
    /// </summary>
    /// <param name="layer"></param>
    public void AddLayer(LBSLayer layer)
    {
        if (layer == null || layers.Contains(layer))
            return;

        layers.Insert(0, layer);
        layer.OnAddModule += (layer, module) => this.OnChanged?.Invoke(this);
        layer.Parent = this;
        this.OnChanged?.Invoke(this);
    }

    /// <summary>
    /// Removes a layer from the list of layers and unsubscribes
    /// the OnChanged event of the removed layer.
    /// </summary>
    /// <param name="layer">The layer to remove</param>
    public void RemoveLayer(LBSLayer layer)
    {
        layers.Remove(layer);
        layer.OnAddModule -= (layer, module) => this.OnChanged(this);
        this.OnChanged?.Invoke(this);
    }

    public void ReplaceLayer(LBSLayer oldLayer, LBSLayer newLayer)
    {
        var index = layers.IndexOf(oldLayer);
        RemoveLayer(oldLayer);
        layers.Insert(index, newLayer);
        this.OnChanged?.Invoke(this);
    }

    public LBSLayer RemoveAt(int index)
    {
        var layer = layers[index];
        layers.RemoveAt(index);
        layer.OnAddModule -= (layer, module) => this.OnChanged(this);
        this.OnChanged?.Invoke(this);
        return layer;
    }

    public override bool Equals(object obj)
    {
        // cast other object
        var other = obj as LBSLevelData;

        // check if other object are the same type
        if (other == null) return false;

        // get amount of layers
        var lCount = other.layers.Count;

        // check if amount of layers are EQUALS
        if (this.layers.Count != lCount) return false;

        // check if contain EQUALS layers
        for (int i = 0; i < lCount; i++)
        {
            var l1 = this.layers[i];
            var l2 = other.layers[i];

            if (!l1.Equals(l2)) return false;
        }

        // check if contain EQUALS quests
        for (int i = 0; i < other.quests.Count; i++)
        {
            var q1 = this.quests[i];
            var q2 = other.quests[i];

            if (!q1.Equals(q2)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }
    #endregion
}

