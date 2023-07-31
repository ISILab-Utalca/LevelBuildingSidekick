using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace LBS.Components.Specifics
{
    [System.Serializable]
    public class RoomData : ICloneable
    {
        static System.Random random = new System.Random();

        #region FIELDS
        [SerializeField, JsonRequired]
        private int width = 1;

        [SerializeField, JsonRequired]
        private int height = 1;

        [ScriptableObjectReference(typeof(LBSIdentifier), "Interior Styles")]
        [SerializeField, JsonRequired]
        private List<string> interiorTags = new List<string>();

        [ScriptableObjectReference(typeof(LBSIdentifier), "Interior Styles")]
        [SerializeField, JsonRequired]
        private List<string> exteriorTags = new List<string>();

        [SerializeField, JsonRequired, JsonConverter(typeof(ColorConverter))]
        private Color color = Color.gray;

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public Color Color
        {
            get => color;
            set => color = value;
        }

        [JsonIgnore]
        public int Width
        {
            get => width;
            set => width = Math.Max(value, 1);
        }

        [JsonIgnore]
        public int Height
        {
            get => height;
            set => height = Math.Max(value, 1);
        }

        [JsonIgnore]
        public Vector2Int Size
        {
            get => new Vector2Int(Width, Height);
        }


        [JsonIgnore]
        public List<string> InteriorTags => new List<string>(interiorTags);

        [JsonIgnore]
        public List<string> ExteriorTags => new List<string>(exteriorTags);

        #endregion

        #region CONSTRUCTORS

        public RoomData()
        {
            this.color = new Color(
                (float)random.NextDouble() * 0.8f,
                (float)random.NextDouble() * 0.8f,
                (float)random.NextDouble() * 0.8f);

            /*
            var ttt = typeof(RoomData).GetField("tags");
            var atts = ttt.GetCustomAttributes(typeof(ScriptableToStringAttribute), false);
            var att = atts[0];
            var _default = (att as ScriptableToStringAttribute).SOs[0];
            tags.Add((_default as LBSIdentifier).Label); // parche para que siempre entre con una tag por dafult (!!!)
            */
        }

        public RoomData(int width, int height, List<string> tags, Color color)
        {
            this.width = width;
            this.height = height;

            this.interiorTags = tags;
            this.color = color;
        }

        #endregion

        #region METHODS

        public bool AddTag(string tag)
        {
            if (interiorTags.Contains(tag))
            {
                return false;
            }
            interiorTags.Add(tag);
            return true;
        }

        public string GetTag(int index)
        {
            if (interiorTags.ContainsIndex(index))
                return interiorTags[index];
            return null;
        }

        public bool Remove(string tag)
        {
            return interiorTags.Remove(tag);
        }

        public string RemoveAt(int index)
        {
            if (!interiorTags.ContainsIndex(index))
                return null;
            var t = interiorTags[index];
            interiorTags.RemoveAt(index);
            return t;
        }

        public object Clone()
        {
            return new RoomData(this.width, this.height, new List<string>(interiorTags), this.color);
        }

        #endregion
    }

}

