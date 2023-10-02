using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class LBSTile : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        protected int x, y;
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);
            set { x = value.x; y = value.y; }
        }
        #endregion

        #region COSNTRUCTORS
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
