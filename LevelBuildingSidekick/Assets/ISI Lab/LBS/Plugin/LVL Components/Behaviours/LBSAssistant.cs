using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Assisstants
{
    [System.Serializable]
    public abstract class LBSAssistant : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [NonSerialized, HideInInspector, JsonIgnore]
        private LBSLayer owner;
        [NonSerialized, HideInInspector, JsonIgnore]
        private Texture2D icon;
        [SerializeField, JsonRequired]
        private string name;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public LBSLayer Owner
        {
            get => owner;
            set => owner = value;
        }

        [JsonIgnore]
        public Texture2D Icon
        {
            get => icon;
        }

        [JsonIgnore]
        public string Name
        {
            get => name;
        }
        #endregion

        #region EVENTS
        [JsonIgnore]
        public Action OnStart;
        [JsonIgnore]
        public Action OnTermination;
        #endregion

        #region CONSTRUCTORS
        public LBSAssistant(Texture2D icon, string name)
        {
            this.icon = icon;
            this.name = name;
        }
        #endregion

        #region METHODS
        public virtual void OnAttachLayer(LBSLayer layer)
        {
            Owner = layer;
        }

        public virtual void OnDetachLayer(LBSLayer layer)
        {
            Owner = null;
        }

        public abstract void Execute();

        public List<Type> GetRequieredModules()
        {
            var toR = new List<Type>();
            Type type = this.GetType();

            object[] atts = type.GetCustomAttributes(true);

            foreach (var att in atts)
            {
                if (att is RequieredModuleAttribute)
                {
                    toR.AddRange((att as RequieredModuleAttribute).types);
                }
            }
            return toR;
        }

        public abstract object Clone();
        #endregion
    }
}
