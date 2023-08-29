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
        protected string id;

        [SerializeField, JsonRequired]
        protected bool visible;

        [SerializeField, JsonRequired]
        protected bool changed;

        [JsonIgnore, HideInInspector]
        private LBSLayer owner;
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public LBSLayer Owner
        {
            get
            {
                return owner;
            }
            set
            {
                owner = value;
            }
        }

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

        #endregion

        #region EVENTS

        [JsonIgnore]
        public Action<LBSModule> OnChanged;

        #endregion

        #region CONSTRUCTOR

        public LBSModule() { ID = GetType().Name; }

        public LBSModule(string key) { ID = key; }

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

        public abstract Rect GetBounds();

        public abstract void Reload(LBSLayer layer);

        public abstract void OnAttach(LBSLayer layer);

        public abstract void OnDetach(LBSLayer layer);

        public abstract void Rewrite(LBSModule module);
        #endregion
    }
}

