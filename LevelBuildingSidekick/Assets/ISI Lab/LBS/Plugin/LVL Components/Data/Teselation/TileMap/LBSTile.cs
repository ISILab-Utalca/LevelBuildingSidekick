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

        //[SerializeField, JsonRequired]
        //protected int sides = 4;

        //[SerializeField, JsonRequired]
        //protected string id;
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x,y);
            set { x = value.x; y = value.y; }
        }

        //[JsonIgnore]
        //public int Sides => sides;

        //[JsonIgnore]
        //public string ID // yo creo que la id en tile es inesesaria ya que el area ya sabe coales son sus tiles
        //{
        //    get => id;
        //    set => id = value;
        //}

        #endregion

        #region COSNTRUCTORS

        public LBSTile(Vector2 position)//, string id, int sides = 4) 
        {
            this.x = (int)position.x;
            this.y = (int)position.y;
            //this.id = id;
            //this.sides = sides; 
        }
        #endregion

        #region METHODS
        public override bool Equals(object obj)
        {
            if(obj is LBSTile)
            {
                var b = Position == (obj as LBSTile).Position;
                return b;
            }
            return false;
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

