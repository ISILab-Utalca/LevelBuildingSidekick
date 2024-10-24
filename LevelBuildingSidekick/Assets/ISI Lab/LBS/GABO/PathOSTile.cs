using ISILab.LBS.Components;
using Newtonsoft.Json;
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
        private bool isDynamicTagTrigger = false;
        [SerializeField, JsonRequired]
        private bool isDynamicObstacleObject = false;
        [SerializeField, JsonRequired]
        private PathOSObstacleConnections obstacles;
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

        #region PROPERTIES
        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public Vector2Int Position { get { return new Vector2Int(x, y); } }
        public PathOSTag Tag { get { return tag; } set { tag = value; } }
        public bool IsDynamicTagObject { get { return isDynamicTagObject; } set { isDynamicTagObject = value; } }
        public bool IsDynamicTagTrigger { get { return isDynamicTagTrigger; } set { isDynamicTagTrigger = value; } }
        public bool IsDynamicObstacleObject { get { return isDynamicObstacleObject; } set { isDynamicObstacleObject = value; } }
        public bool IsDynamicObstacleTrigger
        {
            get { return obstacles != null; }
            set
            {
                if (value)
                {
                    obstacles = new(this, new());
                }
                else
                {
                    obstacles = null;
                }
            }
        }
        #endregion

        // GABO TODO: HACER METODOS PARA ACTIVAR Y DESACTIVAR LOS EVENT TAGS
        #region METHODS
        public void AddObstacle(PathOSTile obstacleTile, PathOSObstacleConnections.Category category)
        {
            // Chequeo de Condiciones
            if (obstacleTile == null) { Debug.LogWarning("Tile obstaculo es nulo!"); return; }
            if (!obstacleTile.isDynamicObstacleObject) { Debug.LogWarning("Tile dado no es obstaculo!"); }
            if (!IsDynamicObstacleTrigger)
            {
                IsDynamicObstacleTrigger = true;
            }            
            obstacles.AddObstacle(obstacleTile, category);
        }

        public void RemoveObstacle(PathOSTile obstacleTile)
        {
            // Chequeo de Condiciones
            if (obstacleTile == null) { Debug.LogWarning("Tile obstaculo es nulo!"); return; }
            if (!obstacleTile.isDynamicObstacleObject) { Debug.LogWarning("Tile dado no es obstaculo!"); }
            if (!IsDynamicObstacleTrigger)
            {
                Debug.LogWarning("Se intenta remover obstaculo de tile que NO es DynamicObstacleTrigger!");
                return;
            }
            obstacles.RemoveObstacle(obstacleTile.X, obstacleTile.Y);
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
