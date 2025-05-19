using System;
using System.Collections.Generic;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    [Serializable]
    public abstract class  LBSBehaviour : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [SerializeField, HideInInspector, JsonIgnore]
        private LBSLayer ownerLayerLayer;
        [SerializeField, JsonRequired] 
        private VectorImage icon;
        [SerializeField, JsonRequired] 
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
            get => ownerLayerLayer;
            set => ownerLayerLayer = value;
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

        #region CONSTRUCTORS
        public LBSBehaviour(VectorImage icon, string name, Color  colorTint)
        {
            this.icon = icon;
            this.name = name;
            this.colorTint = colorTint;
        }
        #endregion

        #region METHODS
        public List<Type> GetRequieredModules()
        {
            var toR = new List<Type>();
            Type tipo = this.GetType();

            object[] atts = tipo.GetCustomAttributes(true);

            foreach (var att in atts)
            {
                if (att is RequieredModuleAttribute)
                {
                    toR.AddRange((att as RequieredModuleAttribute).types);
                }
            }
            return toR;
        }

        public abstract void OnAttachLayer(LBSLayer layer);

        public abstract void OnDetachLayer(LBSLayer layer);

        public abstract object Clone();
        
        public abstract void OnGUI();
        
        #endregion
    }
}