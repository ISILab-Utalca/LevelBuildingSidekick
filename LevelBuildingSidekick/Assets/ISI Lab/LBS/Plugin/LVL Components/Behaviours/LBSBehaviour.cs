using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LBS.Behaviours
{
    [System.Serializable]
    public abstract class LBSBehaviour : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [HideInInspector]
        private LBSLayer owner;
        [SerializeField]
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

        #region CONSTRUCTORS
        public LBSBehaviour(Texture2D icon, string name)
        {
            this.icon = icon;
            this.name = name;
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
        #endregion
    }
}