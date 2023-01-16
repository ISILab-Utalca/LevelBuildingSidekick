using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Specifics
{
    public class RoomData : ICloneable
    {
        //FIELDS
        [SerializeField, JsonRequired]
        private int width = 1;

        [SerializeField, JsonRequired]
        private int height = 1;

        [SerializeField, JsonRequired]
        private List<string> tags = new List<string>();

        //PROPERTIES
        [JsonIgnore]
        public int Width
        {
            get => width;
            set
            {
                if (value >= 1)
                    width = value;
            }
        }

        [JsonIgnore]
        public int Height
        {
            get => height;
            set
            {
                if (value >= 1)
                    height = value;
            }
        }

        [JsonIgnore]
        public int TagCount => tags.Count;

        //METHODS
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
            throw new NotImplementedException();
        }

    }
}

