using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    [System.Serializable]
    public abstract class  LBSBehaviour : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [HideInInspector]
        private LBSLayer owner;
        [SerializeField]
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
        public LBSLayer Owner
        {
            get => owner;
            set => owner = value;
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