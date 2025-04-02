using System;
using System.Collections;
using System.Collections.Generic;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    [System.Serializable]
    public abstract class LBSAssistant : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [SerializeField, HideInInspector, JsonIgnore]
        private LBSLayer ownerLayer;
        [SerializeField, JsonIgnore]//, JsonIgnore]
        private VectorImage icon;
        [SerializeField, JsonIgnore]//, JsonIgnore]
        private Color colorTint;
        [SerializeField, JsonRequired]
        private string name;
        #endregion
        
        #region PROPERTIES
        
        [JsonIgnore]
        public Color ColorTint
        {
            get => colorTint;
            set => colorTint = value;
        }
        
        [JsonIgnore]
        public LBSLayer OwnerLayer
        {
            get => ownerLayer;
            set => ownerLayer = value;
        }

        [JsonIgnore]
        public VectorImage Icon
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
        public LBSAssistant(VectorImage icon, string name, Color colorTint)
        {
            this.icon = icon;
            this.name = name;
            this.colorTint = colorTint;
        }
        #endregion

        #region METHODS
        public virtual void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
        }

        public virtual void OnDetachLayer(LBSLayer layer)
        {
            OwnerLayer = null;
        }

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
        
        public abstract void OnGUI();
        
        #endregion
    }
}
