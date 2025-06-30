using System;
using System.Collections.Generic;
using System.Linq;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

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
        
        private HashSet<object> _keys = new ();
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
        public VectorImage Icon => icon;

        [JsonIgnore]
        public string Name => name;

        [JsonIgnore]
        public HashSet<object> Keys => _keys ??= new HashSet<object>();

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

        public abstract void OnAttachLayer(LBSLayer layer);

        public abstract void OnDetachLayer(LBSLayer layer);

        public abstract object Clone();
        
        public abstract void OnGUI();
        
        #endregion
        
        #region VISUAL ELEMENTS HANDLING METHODS
        /* To optimally handle all the visual elements in display in the MainView of the LBS tool,
         * each behaviour has a HashSet of elements that it wants to draw, and a HashSet of elements
         * it wants to erase, these are _newTiles and _expiredTiles respectively.
        
         * Both of these HashSet use object as value, this object is the instance that will be
         * represented through a VisualElement, it might be a LBSTile, QuestNodeView, or even different
         * classes from the same behaviour. Eventually, all VisualElements are stored in MainView
         * dictionaries, using the defined object as key.
        
         * VisualElements are added and erased from the MainView as the following cycle:
         * Note that tile here is used as any class that is being represented through a VisualElement.
         *
         * 1.- The expired tiles are removed.
         *     MainView gets the _expiredTiles set from all the behaviours in a layer, while clearing
         *     its memory, and erases them if they are in the view.
         *
         * 2.- The Drawers create new tiles.
         *      2.1.- Each Drawer gets the new elements from their respective behaviors, while clearing its
         *            memory.
         *      2.2.- The new VisualElements are created from the retrieved elements, and drawn in the
         *            MainView.
         * 
         * 3.- New tiles, and new expired tiles are saved.
         *     In each behavior, when an action is supposed to create a new VisualElement, a
         *     representative instance is stored for later in the _newTiles set.
         *     If an action is supposed to make a VisualElement disappear, then the class used for its
         *     construction is again stored for later but in the _expiredTiles set.
        */
        
        // These methods are a safer way of adding new objects to the sets
        protected void RequestTilePaint(object tile)
        {
            _keys ??= new HashSet<object>();
            _newTiles ??= new HashSet<object>();

            _newTiles.Add(tile);
            _keys.Add(tile);
        }
        protected bool RequestTileRemove(object tile)
        {
            if (_keys == null)
            {
                _keys = new HashSet<object>();
                return false;
            }

            if (!_keys.Remove(tile)) return false;
            
            _expiredTiles ??= new HashSet<object>();
            _expiredTiles.Add(tile);
            return true;
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
        
        #endregion
    }
}