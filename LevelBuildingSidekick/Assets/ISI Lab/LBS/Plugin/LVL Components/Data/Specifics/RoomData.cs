using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Specifics
{
    [System.Serializable]
    public class RoomData : ICloneable
    {
        #region FIELDS

        [SerializeField, JsonRequired]
        private int width = 1;

        [SerializeField, JsonRequired]
        private int height = 1;

        [SerializeField, JsonRequired]
        [ScriptableToString(typeof(LBSIdentifier))]
        private List<string> tags = new List<string>();

        [SerializeField, JsonRequired]
        public Color color = Color.gray;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = Math.Max(value, 1);
            }
        }

        [JsonIgnore]
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = Math.Max(value, 1);
            }
        }

        [JsonIgnore]
        public Vector2Int Size
        {
            get => new Vector2Int(Width, Height);
            set
            {
                if (value.x >= 1 && value.y >= 1)
                {
                    width = value.x;
                    height = value.y;
                }
            }
        }

        [JsonIgnore]
        public int TagCount => tags.Count;

        [JsonIgnore]
        public List<string> Tags => new List<string>(tags);

        #endregion

        #region CONSTRUCTORS

        public RoomData()
        {

        }

        public RoomData(int width, int height, List<string> tags, Color color)
        {
            this.width = width;
            this.height = height;
            this.tags = tags;
            this.color = color;
        }

        #endregion

        #region METHODS

        public bool AddTag(string tag)
        {
            if (tags.Contains(tag))
            {
                return false;
            }
            tags.Add(tag);
            return true;
        }

        public string GetTag(int index)
        {
            if (tags.ContainsIndex(index))
                return tags[index];
            return null;
        }

        public bool Remove(string tag)
        {
            return tags.Remove(tag);
        }

        public string RemoveAt(int index)
        {
            if (!tags.ContainsIndex(index))
                return null;
            var t = tags[index];
            tags.RemoveAt(index);
            return t;
        }

        public object Clone()
        {
            return new RoomData(this.Width,this.Height,new List<string>(tags), this.color);
        }

        #endregion
    }
}

