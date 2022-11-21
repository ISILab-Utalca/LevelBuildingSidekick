using LBS.Representation.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Representation
{
    [System.Serializable]
    public class TileData : ICloneable
    {
        // Fields
        [SerializeField, JsonRequired]
        private int x;
        [SerializeField, JsonRequired]
        private int y;
        [SerializeField, JsonRequired]
        private int rotation;
        
        // Properties
        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);
            set { x = value.x; y = value.y; }
        }

        // Events
        [HideInInspector]
        public event Action<TileData> OnDataChange;

        // Constructors
        public TileData() { }

        public TileData(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public TileData(Vector2Int pos)
        {
            this.x = pos.x;
            this.y = pos.y;
        }

        // Methods
        public object Clone()
        {
            var clone = new TileData(this.x, this.y);
            return clone;
        }

        [Obsolete("this method is deprecated, instead use the 'Postion' property instead")]
        public Vector2Int GetPosition()
        {
            return new Vector2Int(x, y);
        }

        [Obsolete("this method is deprecated, instead use the 'Position' property instead")]
        public void SetPosition(Vector2Int pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            OnDataChange?.Invoke(this);
        }

        public override bool Equals(object obj)
        {
            var other = (TileData)obj;
            if (other == null)
                return false;

            if (this.x == other.x && this.y == other.y)
                return true;

            return false;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}

