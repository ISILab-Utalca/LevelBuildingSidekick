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

        #region FIELDS

        [SerializeField, JsonRequired]
        protected string key;

        [SerializeField, JsonRequired]
        protected bool visible;

        [SerializeField, JsonRequired]
        protected bool changed;

        [JsonIgnore, HideInInspector]
        public LBSLayer Owner;

        #endregion

        #region PROPERTIES

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

        #endregion

        #region EVENTS

        [JsonIgnore]
        public Action<LBSModule> OnChanged;

        #endregion

        #region CONSTRUCTOR

        public LBSModule() { Key = GetType().Name; }

        public LBSModule(string key) { Key = key; }

        #endregion

        #region METHODS

        /// <summary>
        /// prints by console basic information of 
        /// the representation.
        /// </summary>
        public abstract void Print();

        /// <summary>
        /// Cleans all the information saved in.
        /// </summary>
        public abstract void Clear();

        public abstract bool IsEmpty();

        public abstract object Clone();

        #endregion
    }
}

