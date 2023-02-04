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

    [SerializeField, JsonRequired]
    private List<string> tags = new List<string>(); // (?) unnused

    [SerializeField, JsonRequired]
    private int maxWidth, maxHeight;

    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSLayer> layers = new List<LBSLayer>();

    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public List<LBSLayer> Layers => layers;

    [JsonIgnore]
    public Vector2Int MaxSize
    {
        get => new Vector2Int(maxWidth, maxHeight);
        set
        {
            maxWidth = (int)value.x;
            maxHeight = (int)value.y;
        }
    }

    [JsonIgnore]
    public int LayerCount => layers.Count;

    #endregion

    //EVENTS
    [JsonIgnore]
    public Action<LBSLevelData> OnChanged;

    #region METHODS

    public LBSLayer GetLayer(int index)
    {
        return layers[index];
    }

    public void RemoveNullLayers() // (!) parche, no deberia poder añadirse nulls
    {
        var r = new List<LBSLayer>();
        foreach (var layer in layers)
        {
            if (layer != null)
                r.Add(layer);
        }
        layers = r;
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

        layers.Add(layer);
        layer.OnChanged += (l) => this.OnChanged(this);
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
        layer.OnChanged -= (l) => this.OnChanged(this);
        this.OnChanged?.Invoke(this);
    }

    public LBSLayer RemoveAt(int index)
    {
        var layer = layers[index];
        layers.RemoveAt(index);
        layer.OnChanged -= (l) => this.OnChanged(this);
        this.OnChanged?.Invoke(this);
        return layer;
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
            return layers.Find( l => l.ID == id);
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
        }

        return null;
    }

    #endregion
}

