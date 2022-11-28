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
        private int rotation; // (!) estos tiles no necesitan rotacion, son los 3d los que si, estos simplemente pueden cambiar sus etiquetas de lugar
        [SerializeField, JsonRequired]
        private string[] connection = new string[4]; // (?) name can also be "relationshipNeighbors"

        // Properties
        [HideInInspector, JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);
            set { x = value.x; y = value.y; }
        }
        [HideInInspector, JsonIgnore]
        public int Rotation
        {
            get => rotation;
            set
            {
                if (!CanBeRotated)
                    return;
                rotation = Mathf.Clamp(value, 0, connection.Length);
            }
        }
        [HideInInspector, JsonIgnore]
        public bool CanBeRotated => true;

        // Events
        [HideInInspector]
        public event Action<TileData> OnDataChange;

        // Constructors
        public TileData() { }

        public TileData(int x, int y,int rotation, string[] sideConections)
        {
            this.x = x;
            this.y = y;
            this.rotation = rotation;
            this.connection = sideConections;
        }

        public TileData(Vector2Int pos,int rotation, string[] sideConections)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.rotation = rotation;
            this.connection = sideConections;
        }

        // Methods
        public object Clone()
        {
            var c = new string[this.connection.Length];
            for (int i = 0; i < connection.Length; i++)
            {
                c[i] = connection[i];
            }
            var clone = new TileData(this.x, this.y, this.rotation, c);
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

        internal void SetConection(int index, string value)
        {
            connection[index] = value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TileData))
                return false;

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

