using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class LBSTile : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        public int x, y;

        //NOTA: El tag estaba causando problemas con la serialización. Puede que cause problemas a futuro.
        //Si se necesita reimplementar, debería reimplementarse en ExteriorDrawer (el script que lo utiliza) y no en LBSTile.

        //public LBSTag tag;
        
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);
            set { x = value.x; y = value.y; }
        }
        #endregion

        #region CONSTRUCTORS
        public LBSTile(Vector2 position)
        {
            this.x = (int)position.x;
            this.y = (int)position.y;
        }
        #endregion

        #region METHODS
        public override bool Equals(object obj)
        {
            var other = obj as LBSTile;

            if (other == null) return false;

            if (!other.Position.Equals(this.Position)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual object Clone()
        {
            return new LBSTile(Position);
        }
        #endregion
    }
}
