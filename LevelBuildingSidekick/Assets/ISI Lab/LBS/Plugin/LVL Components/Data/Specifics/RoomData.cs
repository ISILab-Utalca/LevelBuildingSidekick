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

        [ScriptableToString(typeof(LBSIdentifier))]
        [SerializeField, JsonRequired]
        private List<string> tags = new List<string>();

        [SerializeField, JsonRequired]
        private SerializableColor color = Color.gray.ToSerializable();

        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public Color Color
        {
            get => color.ToColor();
            set => color = value.ToSerializable();
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
        public int TagCount => tags.Count;

        [JsonIgnore]
        public List<string> Tags => new List<string>(tags);

        #endregion

        #region CONSTRUCTORS

        public RoomData()
        {
            this.color = new Color(
                (float)random.NextDouble() * 0.8f,
                (float)random.NextDouble() * 0.8f,
                (float)random.NextDouble() * 0.8f)
                .ToSerializable();

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
            this.tags = tags;
            this.color = color.ToSerializable();
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
            return new RoomData(this.width, this.height, new List<string>(tags), this.color.ToColor());
        }

        #endregion
    }

}

