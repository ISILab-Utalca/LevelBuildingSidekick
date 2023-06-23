using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class WallData : ICloneable // esto no corresponde a una muralla como tal sino que a los tiles de borde de una habitacion
{
    #region FIELDS

    [SerializeField, JsonRequired, SerializeReference]
    protected string ownerID;

    [SerializeField, JsonRequired, SerializeReference]
    protected Vector2Int dir; // direccion a la que mira con respecto a su habitacion

    [SerializeField, JsonRequired, SerializeReference]
    protected List<Vector2Int> allTiles;

    #endregion

    #region PROPERTIES

    [JsonIgnore]
    public Vector2Int First => allTiles.First();

    [JsonIgnore]
    public Vector2Int Last => allTiles.Last();

    [JsonIgnore]
    public string OwnerID => ownerID;

    [JsonIgnore]
    public Vector2Int Dir => dir;

    [JsonIgnore]
    public List<Vector2Int> Tiles => allTiles;

    [JsonIgnore]
    public float Length => (Last - First).magnitude; 

    #endregion

    #region CONSTRUCTOR

    public WallData()
    {
        dir = Vector2Int.zero;
        allTiles = new List<Vector2Int>();
    }

    public WallData(string ownerID, Vector2Int dir, List<Vector2Int> allTiles = null)
    {
        this.ownerID = ownerID;
        this.dir = dir;
        this.allTiles = allTiles;
    }

    #endregion

    #region METHODS
    public object Clone() // (!!) implementar
    {
        var w = new WallData();
        w.dir = Dir;
        w.allTiles = new List<Vector2Int>(allTiles);
        return w;
    }

    #endregion
}
