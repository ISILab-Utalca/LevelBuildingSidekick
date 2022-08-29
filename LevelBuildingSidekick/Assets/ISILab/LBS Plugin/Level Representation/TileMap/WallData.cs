using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Representation.TileMap
{

    [Serializable]
    internal class WallData : ICloneable // esto no corresponde a una muralla como tal sino que a los tiles de borde de una habitacion
    {
        // info
        internal string ownerID;
        internal Vector2Int firstCorner;
        internal Vector2Int secondCorner;

        // Metainfo
        internal Vector2Int dir; // direccion a la que mira con respecto a su habitacion
        internal List<Vector2Int> allTiles;

        public WallData(Vector2Int corner1, Vector2Int corner2, string ownerID, Vector2Int dir, List<Vector2Int> allTiles = null)
        {
            this.firstCorner = corner1;
            this.secondCorner = corner2;
            this.ownerID = ownerID;

            this.dir = dir;
            this.allTiles = allTiles;
        }

        public object Clone()
        {
            return null;
        }
    }
}