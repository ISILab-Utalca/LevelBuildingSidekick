using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    //GABO TODO
    public class PathOSModule : LBSModule
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private List<PathOSTile> tiles = new List<PathOSTile>();
        #endregion

        #region EVENTS
        public event Action<PathOSModule, PathOSTile> OnAddTile;
        public event Action<PathOSModule, PathOSTile> OnRemoveTile;
        #endregion

        #region METHODS

        public void AddTile(PathOSTile tile)
        {
            // Chequeo nulo
            if (tile == null) { Debug.LogWarning("PathOSModule.AddTile(): Tile nulo!"); return; }

            // Remocion tile antiguo (si existe) y agregar nuevo
            var t = GetTile(tile.X, tile.Y);
            if (t != null)
            {
                tiles.Remove(t);
            }
            tiles.Add(tile);

            // Eventos
            OnAddTile?.Invoke(this, tile);
            OnChanged?.Invoke(this, null, new List<object>() { tile });
        }

        public void RemoveTile(PathOSTile tile)
        {
            // Chequeo nulo
            if (tile == null) { Debug.LogWarning("PathOSModule.RemoveTile(): Tile nulo!"); return; }

            var t = GetTile(tile.X, tile.Y);
            if (t != null)
            {
                tiles.Remove(t);

                // Eventos
                OnRemoveTile?.Invoke(this, tile);
                OnChanged?.Invoke(this, null, new List<object>() { tile });
            }
            else
            {
                Debug.LogWarning("PathOSModule.RemoveTile(): No se encuentra el tile a remover!");
            }
        }

        public PathOSTile GetTile(int x, int y)
        {
            if (tiles.Count <= 0)
                return null;
            return tiles.Find(t => t.X == x && t.Y == y);
        }

        public List<PathOSTile> GetTiles()
        {
            return tiles;
        }

        public override void Clear()
        {
            Debug.Log("Ejecutando PathOSModule.Clear()");
            tiles.Clear();
        }

        public override object Clone()
        {
            Debug.Log("Ejecutando PathOSModule.Clone()");
            var clone = new PathOSModule();
            return clone;
        }

        public override bool IsEmpty()
        {
            Debug.Log("Ejecutando PathOSModule.IsEmpty()");
            return (tiles.Count == 0);
        }
        #endregion
    }
}

