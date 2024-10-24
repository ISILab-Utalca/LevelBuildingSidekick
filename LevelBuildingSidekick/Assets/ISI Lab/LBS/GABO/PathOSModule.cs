using ISILab.LBS.Components;
using Newtonsoft.Json;
using System;
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
        public event Action<PathOSModule, PathOSTile> OnApplyEventTile;
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

        // Para aplicar Event Tiles sobre un tile existente.
        public void ApplyEventTile(PathOSTile eventTile)
        {
            // Chequeo nulo
            if (eventTile == null) { Debug.LogWarning("PathOSModule.ApplyEventTile(): Tile nulo!"); return; }
            // Chequeo tag nulo
            if (eventTile.Tag == null) { Debug.LogWarning("PathOSModule.ApplyEventTile(): Tile tiene tag nulo!"); }
            // Chequeo Event Tile
            if (eventTile.Tag.Category != PathOSTag.PathOSCategory.EventTag)
            {
                Debug.LogWarning("PathOSModule.ApplyEventTile(): Tile dado no contiene un Event Tag!"); return;
            }

            // Si existe Tile con Element Tag en la posicion, activa el booleano correspondiente al Event Tag.
            var t = GetTile(eventTile.X, eventTile.Y);
            if (t != null)
            {
                PathOSTag eventTag = eventTile.Tag;
                switch (eventTag.Label)
                {
                    case "DynamicTagObject":
                        t.IsDynamicTagObject = !t.IsDynamicTagObject; break;
                    case "DynamicTagTrigger":
                        t.IsDynamicTagTrigger = !t.IsDynamicTagTrigger; break;
                    case "DynamicObstacleObject":
                        t.IsDynamicObstacleObject = !t.IsDynamicObstacleObject; break;
                    case "DynamicObstacleTrigger":
                        t.IsDynamicObstacleTrigger = !t.IsDynamicObstacleTrigger; break;
                    default:
                        Debug.LogWarning("Tile no contiene un Event Tag valido!"); return;
                }
            }
            else
            {
                Debug.LogWarning("No existe en aquella posicion un tile donde colocar Event Tags.");
            }

            OnChanged?.Invoke(this, null, new List<object>() { eventTile });
            OnApplyEventTile?.Invoke(this, eventTile);
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

