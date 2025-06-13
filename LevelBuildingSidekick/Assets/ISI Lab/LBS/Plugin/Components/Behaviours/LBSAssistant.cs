using System;
using System.Collections.Generic;
using System.Linq;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    [Serializable]
    public abstract class LBSAssistant : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        public bool visible = true;
        #endregion

        #region FIELDS
        [SerializeField, HideInInspector, JsonIgnore]
        private LBSLayer ownerLayer;
        [SerializeField, JsonRequired]//, JsonIgnore]
        private VectorImage icon;
        [SerializeField, JsonRequired]//, JsonIgnore]
        private Color colorTint;
        [SerializeField, JsonRequired]
        private string name;
        
        private HashSet<LBSTile> _newTiles = new ();
        private HashSet<LBSTile> _expiredTiles = new ();
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
        
        
        /// <summary>
        /// Get all new tiles' position that have been created since the last time they were retrieved.
        /// The memory of new tiles will be cleared after calling this method.
        /// </summary>
        public LBSTile[] RetrieveNewTiles()
        {
            // If null create a new one
            _newTiles ??= new HashSet<LBSTile>();
            
            // Turn into array
            LBSTile[] o = _newTiles.ToArray();
            
            // Clear memory
            _newTiles.Clear();
            
            // Return array
            return o;
        }
        
        /// <summary>
        /// Get all tiles' position that   since the last time they were retrieved.
        /// The memory of new tiles will be cleared after calling this method.
        /// </summary>
        public LBSTile[] RetrieveExpiredTiles()
        {
            // If null create a new one
            _expiredTiles ??= new HashSet<LBSTile>();
            
            // Turn into array
            LBSTile[] o = _expiredTiles.ToArray();
            
            // Clear memory
            _expiredTiles.Clear();
            
            // Return array
            return o;
        }

        public abstract object Clone();
        
        public abstract void OnGUI();
        
        #endregion
    }
}
