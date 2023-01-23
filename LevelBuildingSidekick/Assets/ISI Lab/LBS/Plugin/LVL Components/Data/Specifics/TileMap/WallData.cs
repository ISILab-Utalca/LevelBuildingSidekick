using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
internal class WallData : ICloneable // esto no corresponde a una muralla como tal sino que a los tiles de borde de una habitacion
{
    // Fields
    internal string ownerID;
    internal Vector2Int dir; // direccion a la que mira con respecto a su habitacion
    internal List<Vector2Int> allTiles;

    // Properties
    public Vector2Int First => allTiles.First();
    public Vector2Int Last => allTiles.Last();

    // Constructors
    public WallData(string ownerID, Vector2Int dir, List<Vector2Int> allTiles = null)
    {
        this.ownerID = ownerID;
        this.dir = dir;
        this.allTiles = allTiles;
    }

    // Methods
    public object Clone() // (!!) implementar
    {
        throw new System.NotImplementedException();
    }
}
