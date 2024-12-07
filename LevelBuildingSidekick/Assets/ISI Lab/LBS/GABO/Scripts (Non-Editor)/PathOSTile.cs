using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

// GABO TODO: FALTA VER COMO TRATAR ELEMENT TAGS Y EVENT TAGS.
namespace ISILab.LBS.Modules
{
    [System.Serializable]
    public class PathOSTile
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired]
        private PathOSBehaviour owner;
        [SerializeField, JsonRequired]
        private int x, y;
        [SerializeField, JsonRequired]
        private PathOSTag tag;
        // Booleanos para Event Tags
        [SerializeField, JsonRequired]
        private bool isDynamicTagObject = false;
        [SerializeField, JsonRequired]
        private bool isDynamicObstacleObject = false;
        [SerializeField]
        private PathOSObstacleConnections obstacles;
        [SerializeField]
        private PathOSDynamicTagConnections dynamicTagTiles;
        #endregion

        #region CONSTRUCTORS
        public PathOSTile(PathOSBehaviour owner, int x, int y, PathOSTag tag = null)
        {
            this.owner = owner;
            this.x = x;
            this.y = y;
            obstacles = new PathOSObstacleConnections(isNull: true);
            dynamicTagTiles = new PathOSDynamicTagConnections(isNull: true);
            if (tag != null) { this.tag = tag; }
        }
        #endregion

        #region EVENTS
        public Action OnAddObstacle;
        public Action OnRemoveObstacle;
        public Action OnAddDynamicTag;
        public Action OnRemoveDynamicTag;
        public Action OnConvertingToObstacleTrigger;
        public Action OnConvertingToObstacleObject;
        public Action OnConvertingToDynamicTagTrigger;
        public Action OnConvertingToDynamicTagObject;
        public Action OnRevertingFromObstacleTrigger;
        public Action OnRevertingFromObstacleObject;
        public Action OnRevertingFromDynamicTagTrigger;
        public Action OnRevertingFromDynamicTagObject;
        #endregion

        #region PROPERTIES
        public PathOSBehaviour Owner { get { return owner; } set { Owner = value; } }
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }
        public Vector2Int Position { get { return new Vector2Int(x, y); } }
        public PathOSTag Tag { get { return tag; } set { tag = value; } }
        public bool IsDynamicTagObject
        {
            get { return isDynamicTagObject; }
            set
            {
                bool lastValue = isDynamicTagObject;

                isDynamicTagObject = value;

                // Eventos de conversion y reversion (solo si cambio es no redundante)
                if (value && value != lastValue) { OnConvertingToDynamicTagObject?.Invoke(); }
                else if (!value && value != lastValue) { OnRevertingFromDynamicTagObject?.Invoke(); }
            }
        }
        public bool IsDynamicObstacleObject
        {
            get { return isDynamicObstacleObject; }
            set
            {
                bool lastValue = isDynamicObstacleObject;

                isDynamicObstacleObject = value;

                // Eventos de conversion y reversion (solo si cambio es no redundante)
                if (value && value != lastValue) { OnConvertingToObstacleObject?.Invoke(); }
                else if (!value && value != lastValue) { OnRevertingFromObstacleObject?.Invoke(); }
            }
        }
        public bool IsDynamicTagTrigger
        {
            get { return !dynamicTagTiles.IsNull; }
            set
            {
                bool lastValue = !dynamicTagTiles.IsNull;

                if (value)
                {
                    // Solo instanciar si no existe
                    if (dynamicTagTiles.IsNull)
                    {
                        dynamicTagTiles = new(this, new());
                    }
                }
                else
                {
                    dynamicTagTiles = new PathOSDynamicTagConnections(isNull: true);
                }

                // Eventos de conversion y reversion (solo si cambio es no redundante)
                if (value && value != lastValue) { OnConvertingToDynamicTagTrigger?.Invoke(); }
                else if (!value && value != lastValue) { OnRevertingFromDynamicTagTrigger?.Invoke(); }
            }
        }
        public bool IsDynamicObstacleTrigger
        {
            get { return !obstacles.IsNull; }
            set
            {
                bool lastValue = !obstacles.IsNull;

                if (value)
                {
                    // Solo instanciar si no existe
                    if (obstacles.IsNull)
                    {
                        obstacles = new(this, new());
                    }
                }
                else
                {
                    obstacles = new PathOSObstacleConnections(isNull: true);
                }

                // Eventos de conversion y reversion (solo si cambio es no redundante)
                if (value && value != lastValue) { OnConvertingToObstacleTrigger?.Invoke(); }
                else if (!value && value != lastValue) { OnRevertingFromObstacleTrigger?.Invoke(); }
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

        public (PathOSTile, PathOSObstacleConnections.Category)? GetObstacle(int x, int y)
        {
            if (obstacles == null) { return null; }
            return obstacles.GetObstacle(x, y);
        }

        public (PathOSTile, PathOSObstacleConnections.Category)? GetObstacle(PathOSTile tile)
        {
            if (obstacles == null) { return null; }
            return obstacles.GetObstacle(tile);
        }

        public (PathOSTile, PathOSTag)? GetDynamicTag(int x, int y)
        {
            if (dynamicTagTiles == null) { return null; }
            return dynamicTagTiles.GetDynamicTag(x, y);
        }
        public (PathOSTile, PathOSTag)? GetDynamicTag(PathOSTile tile)
        {
            if (dynamicTagTiles == null) { return null; }
            return dynamicTagTiles.GetDynamicTag(tile);
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

        // *NOTA*: Segundo parametro indica si revisar condicion "IsDynamicObstacleObject".
        // Usado en "PathOSModule.CleanAllObstacleConnectionsTo" ya que su invocacion por suscripcion es
        // posterior al seteo de la propiedad en "False".
        public void RemoveObstacle(PathOSTile obstacleTile, bool checkIfObstacleObjectProperty = true)
        {
            // Chequeo de Condiciones
            if (obstacleTile == null) { Debug.LogWarning("Tile obstaculo es nulo!"); return; }
            if (!obstacleTile.isDynamicObstacleObject && checkIfObstacleObjectProperty) { Debug.LogWarning("Tile dado no es obstaculo!"); return; }
            if (!IsDynamicObstacleTrigger)
            {
                Debug.LogWarning("Este tile NO es DynamicObstacleTrigger!");
                return;
            }
            // Chequeo de existencia en mapa
            var currConnection = obstacles.GetObstacle(obstacleTile.x, obstacleTile.y);
            if (currConnection == null) { Debug.LogWarning("No existe tile en la posicion!"); return; }
            if (obstacleTile.Tag.Label != currConnection?.Item1.Tag.Label)
            {
                Debug.LogWarning("Tag.Label del tile a remover es distinto del existente!");
                return;
            }

            obstacles.RemoveObstacle(obstacleTile.X, obstacleTile.Y);

            OnRemoveObstacle?.Invoke();
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

            OnAddDynamicTag?.Invoke();
        }

        // *NOTA*: Segundo parametro indica si revisar condicion "IsDynamicTagObject".
        // Usado en "PathOSModule.CleanAllDynamicTagConnectionsTo" ya que su invocacion por suscripcion es
        // posterior al seteo de la propiedad en "False".
        public void RemoveDynamicTag(PathOSTile tagTile, bool checkIfDynamicTagObjectProperty = true)
        {
            // Chequeos
            if (tagTile == null) { Debug.LogWarning("Tag tile es nulo!"); return; }
            if (!tagTile.isDynamicTagObject && checkIfDynamicTagObjectProperty) { Debug.LogWarning("Tile dado no es DynamicTileObject!"); return; }
            if (!IsDynamicTagTrigger)
            {
                Debug.LogWarning("Este tile NO es DynamicTagTrigger!");
                return;
            }
            // Chequeo de existencia en mapa
            var currConnection = dynamicTagTiles.GetDynamicTag(tagTile.x, tagTile.y);
            if (currConnection == null) { Debug.LogWarning("No existe tile en la posicion!"); return; }
            if (tagTile.Tag.Label != currConnection?.Item1.Tag.Label)
            {
                Debug.LogWarning("Tag.Label del tile a remover es distinto del existente!");
                return;
            }

            dynamicTagTiles.RemoveDynamicTag(tagTile.X, tagTile.Y);

            OnRemoveDynamicTag?.Invoke();
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
