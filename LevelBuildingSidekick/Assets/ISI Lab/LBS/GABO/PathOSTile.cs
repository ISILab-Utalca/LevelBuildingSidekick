using ISILab.LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

// GABO TODO: FALTA VER COMO TRATAR ELEMENT TAGS Y EVENT TAGS.
namespace ISILab.LBS.Modules
{
    public class PathOSTile
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private int x, y;
        [SerializeField, JsonRequired]
        private PathOSTag tag;
        // Booleanos para Event Tags
        [SerializeField, JsonRequired]
        private bool isDynamicTagObject = false;
        [SerializeField, JsonRequired]
        private bool isDynamicObstacleObject = false;
        [SerializeField, JsonRequired]
        private PathOSObstacleConnections obstacles;
        [SerializeField, JsonRequired]
        private PathOSDynamicTagConnections dynamicTagTiles;
        #endregion

        #region CONSTRUCTORS
        public PathOSTile(int x, int y, PathOSTag tag = null)
        {
            this.x = x;
            this.y = y;
            obstacles = null;
            if (tag != null) { this.tag = tag; }
        }
        #endregion

        #region EVENTS
        public Action OnAddObstacle;
        #endregion

        #region PROPERTIES
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public Vector2Int Position { get { return new Vector2Int(x, y); } }
        public PathOSTag Tag { get { return tag; } set { tag = value; } }
        public bool IsDynamicTagObject { get { return isDynamicTagObject; } set { isDynamicTagObject = value; } }
        public bool IsDynamicTagTrigger
        {
            get { return dynamicTagTiles != null; }
            set
            {
                if (value)
                {
                    // Solo instanciar si no existe
                    if (dynamicTagTiles == null)
                    {
                        dynamicTagTiles = new(this, new());
                    }
                }
                else
                {
                    dynamicTagTiles = null;
                }
            }
        }
        public bool IsDynamicObstacleObject { get { return isDynamicObstacleObject; } set { isDynamicObstacleObject = value; } }
        public bool IsDynamicObstacleTrigger
        {
            get { return obstacles != null; }
            set
            {
                if (value)
                {
                    // Solo instanciar si no existe
                    if (obstacles == null)
                    {
                        obstacles = new(this, new());
                    }
                }
                else
                {
                    obstacles = null;
                }
            }
        }
        #endregion

        #region METHODS
        public List<(PathOSTile, PathOSObstacleConnections.Category)> GetObstacles()
        {
            // Chequeo de existencia.
            if (obstacles == null) return null;
            return obstacles.Obstacles;
        }

        public List<(PathOSTile, PathOSTag)> GetDynamicTags()
        {
            return dynamicTagTiles.DynamicTagObjects;
        }

        public (PathOSTile, PathOSObstacleConnections.Category) GetObstacle(int x, int y)
        {
            return obstacles.GetObstacle(x, y);
        }

        public (PathOSTile, PathOSObstacleConnections.Category) GetObstacle(PathOSTile tile)
        {
            return obstacles.GetObstacle(tile);
        }

        public (PathOSTile, PathOSTag) GetDynamicTag(int x, int y)
        {
            return dynamicTagTiles.GetDynamicTag(x, y);
        }

        public void AddObstacle(PathOSTile obstacleTile, PathOSObstacleConnections.Category category)
        {
            // Chequeo de Condiciones
            if (obstacleTile == null) { Debug.LogWarning("Tile obstaculo es nulo!"); return; }
            if (!obstacleTile.isDynamicObstacleObject) { Debug.LogWarning("Tile dado no es obstaculo!"); return; }
            if (!IsDynamicObstacleTrigger)
            {
                IsDynamicObstacleTrigger = true;
            }
            obstacles.AddObstacle(obstacleTile, category);

            OnAddObstacle?.Invoke();
        }

        public void RemoveObstacle(PathOSTile obstacleTile)
        {
            // Chequeo de Condiciones
            if (obstacleTile == null) { Debug.LogWarning("Tile obstaculo es nulo!"); return; }
            if (!obstacleTile.isDynamicObstacleObject) { Debug.LogWarning("Tile dado no es obstaculo!"); return; }
            if (!IsDynamicObstacleTrigger)
            {
                Debug.LogWarning("Este tile NO es DynamicObstacleTrigger!");
                return;
            }
            // Chequeo de existencia en mapa
            var currConnection = obstacles.GetObstacle(obstacleTile.x, obstacleTile.y);
            if (currConnection == (null, null)) { Debug.LogWarning("No existe tile en la posicion!"); return; }
            if (obstacleTile.Tag.Label != currConnection.Item1.Tag.Label)
            {
                Debug.LogWarning("Tag.Label del tile a remover es distinto del existente!");
                return;
            }

            obstacles.RemoveObstacle(obstacleTile.X, obstacleTile.Y);
        }

        public void AddDynamicTag(PathOSTile tagTile, PathOSTag tag)
        {
            // Chequeo de Condiciones
            if (tagTile == null) { Debug.LogWarning("Tag tile es nulo!"); return; }
            if (!tagTile.isDynamicTagObject) { Debug.LogWarning("Tile dado no es DynamicTagObject!"); return; }
            if (!IsDynamicTagTrigger)
            {
                IsDynamicTagTrigger = true;
            }
            dynamicTagTiles.AddDynamicTag(tagTile, tag);
        }

        public void RemoveDynamicTag(PathOSTile tagTile)
        {
            // Chequeos
            if (tagTile == null) { Debug.LogWarning("Tag tile es nulo!"); return; }
            if (!tagTile.isDynamicTagObject) { Debug.LogWarning("Tile dado no es DynamicTileObject!"); return; }
            if (!IsDynamicTagTrigger)
            {
                Debug.LogWarning("Este tile NO es DynamicTagTrigger!");
                return;
            }
            // Chequeo de existencia en mapa
            var currConnection = dynamicTagTiles.GetDynamicTag(tagTile.x, tagTile.y);
            if (currConnection == (null, null)) { Debug.LogWarning("No existe tile en la posicion!"); return; }
            if (tagTile.Tag.Label != currConnection.Item1.Tag.Label)
            {
                Debug.LogWarning("Tag.Label del tile a remover es distinto del existente!");
                return;
            }

            dynamicTagTiles.RemoveDynamicTag(tagTile.X, tagTile.Y);
        }
        #endregion

        #region NOT_IN_USE
        //public void AddTag(PathOSTag tag)
        //{
        //    // Chequeo nulo
        //    if (tag == null) { Debug.LogWarning("PathOSTile.AddTag(): Tag nulo!"); return; }

        //    // Remocion tag antiguo (si existe) y agregar nuevo
        //    var t = GetTag(tag);
        //    if (t != null)
        //    {
        //        tags.Remove(t);
        //    }
        //    tags.Add(tag);
        //}

        //public PathOSTag GetTag(PathOSTag tag)
        //{
        //    if (tags.Count <= 0)
        //        return null;
        //    return tags.Find(t => t.Label.Equals(tag.Label));

        //}

        //public void RemoveTag(PathOSTag tag)
        //{
        //    // Chequeo nulo
        //    if (tag == null) { Debug.LogWarning("PathOSTile.RemoveTag(): Tag nulo!"); return; }

        //    var t = GetTag(tag);
        //    if (t != null)
        //    {
        //        tags.Remove(t);
        //    }
        //}
        #endregion


    }
}
