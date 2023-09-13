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
    private int width;
    [SerializeField, JsonRequired]
    private int height;

    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSLayer> layers = new List<LBSLayer>();

    [SerializeField, JsonRequired, SerializeReference]
    private List<LBSQuest> quest = new List<LBSQuest>();

    #endregion

    #region PROPERTIES

    [JsonIgnore]
    //public List<LBSLayer> Layers => new List<LBSLayer>(layers);
    public List<LBSLayer> Layers => layers;

    [JsonIgnore]
    public Vector2Int MaxSize
    {
        get => new Vector2Int(width, height);
        set
        {
            width = (int)value.x;
            height = (int)value.y;
        }
    }

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
            layer.OnAddModule += (layer, module) => this.OnChanged(this);
            layer.Parent = this;
        }
    }

    public LBSLayer GetLayer(int index)
    {
        return layers[index];
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
        layer.OnAddModule += (layer, module) => this.OnChanged(this);
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

    public void ReplaceLayer(LBSLayer oldLayer,LBSLayer newLayer)
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

