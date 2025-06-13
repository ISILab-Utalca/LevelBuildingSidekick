using System;
using System.Collections.Generic;
using System.Linq;
using LBS.Components;
using LBS.Components.TileMap;
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
        
        private HashSet<object> _newTiles = new ();
        private HashSet<object> _expiredTiles = new ();
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
        public List<Type> GetRequiredModules()
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

        protected void RequestTilePaint(object tile)
        {
            _newTiles ??= new HashSet<object>();

            _newTiles.Add(tile);
        }
        protected void RequestTileRemove(object tile)
        {
            _expiredTiles ??= new HashSet<object>();
            
            _expiredTiles.Add(tile);
        }
        
        
        /// <summary>
        /// Get all new tiles' position that have been created since the last time they were retrieved.
        /// The memory of new tiles will be cleared after calling this method.
        /// </summary>
        public virtual object[] RetrieveNewTiles()
        {
            // If null create a new one
            _newTiles ??= new HashSet<object>();
            
            // Turn into array
            object[] o = _newTiles.ToArray();
            
            // Clear memory
            _newTiles.Clear();
            
            // Return array
            return o;
        }
        
        /// <summary>
        /// Get all tiles' position that   since the last time they were retrieved.
        /// The memory of new tiles will be cleared after calling this method.
        /// </summary>
        public virtual object[] RetrieveExpiredTiles()
        {
            // If null create a new one
            _expiredTiles ??= new HashSet<object>();
            
            // Turn into array
            object[] o = _expiredTiles.ToArray();
            
            // Clear memory
            _expiredTiles.Clear();
            
            // Return array
            return o;
        }

        public abstract void OnAttachLayer(LBSLayer layer);

        public abstract void OnDetachLayer(LBSLayer layer);

        public abstract object Clone();
        
        public abstract void OnGUI();
        
        #endregion
    }
}