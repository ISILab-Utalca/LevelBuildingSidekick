using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    [System.Serializable]
    public abstract class LBSModule
    {
        //FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        string id;

        [SerializeField, JsonRequired, SerializeReference]
        bool visible;

        [SerializeField, JsonRequired, SerializeReference]
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
        public string ID
        {
            get => id;
            set => id = value;
        }

        //EVENTS
        public Action OnChanged;

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
    }
}

