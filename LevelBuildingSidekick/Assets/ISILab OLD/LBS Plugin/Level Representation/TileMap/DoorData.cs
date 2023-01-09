using Newtonsoft.Json;
using System;
using UnityEngine;

namespace LBS.Representation
{
    public class DoorData : ICloneable
    {
        // Fields
        [SerializeField, JsonRequired]
        private int x1, y1;
        [SerializeField, JsonRequired]
        private int x2, y2;

        // Events
        [HideInInspector, JsonIgnore]
        private Action<DoorData> OnDataChange;

        // Constructors
        public DoorData() { }

        public DoorData( Vector2Int pos1, Vector2Int pos2) 
        {
            this.x1 = pos1.x;
            this.y1 = pos1.y;
            this.x2 = pos2.x;
            this.y2 = pos2.y;
        }

        // Methods
        public Vector2Int GetFirstPosition()
        {
            return new Vector2Int(x1, y1);
        }

        public Vector2Int GetSecondPosition()
        {
            return new Vector2Int(x2, y2);
        }

        public object Clone()
        {
            var clone = new DoorData( new Vector2Int(x1, y1), new Vector2Int(x2, y2));
            return clone;
        }

        public override bool Equals(object obj)
        {
            var other = (DoorData)obj;
            if (other == null)
                return false;

            if (this.x1 == other.x1 && this.y1 == other.y1 && this.x2 == other.x2 && this.y2 == other.y2)
                return true;

            if (this.x1 == other.x2 && this.y1 == other.y2 && this.x2 == other.x1 && this.y2 == other.y1)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( x1, y1, x2, y2);
        }

    }
}