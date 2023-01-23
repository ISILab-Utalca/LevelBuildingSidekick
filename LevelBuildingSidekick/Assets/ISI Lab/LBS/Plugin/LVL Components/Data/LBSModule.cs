using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    [System.Serializable]
    public abstract class LBSModule : ICloneable
    {

        //FIELDS
        [SerializeField, JsonRequired]
        string key;

        [SerializeField, JsonRequired]
        bool visible;

        [SerializeField, JsonRequired]
        bool changed;

        //PROPERTIES
        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        [JsonIgnore]
        public bool HasChanged
        {
            get => changed;
            set => changed = value;
        }

        [JsonIgnore]
        public string Key
        {
            get => key;
            set => key = value;
        }

        //EVENTS
        public Action<LBSModule> OnChanged;

        //METHODS

        /// <summary>
        /// prints by console basic information of 
        /// the representation.
        /// </summary>
        public abstract void Print();

        /// <summary>
        /// Cleans all the information saved in.
        /// </summary>
        public abstract void Clear();
        public abstract object Clone();
    }
}

