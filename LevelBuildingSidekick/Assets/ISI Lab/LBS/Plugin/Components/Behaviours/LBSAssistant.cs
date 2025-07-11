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
        
        #region VISUAL ELEMENTS HANDLING METHODS
        /* To optimally handle all the visual elements in display in the MainView of the LBS tool,
         * each assistant has a HashSet of elements that it wants to draw, and a HashSet of elements
         * it wants to erase, these are _newTiles and _expiredTiles respectively.
        
         * Both of these HashSet use object as value, this object is the instance that will be
         * represented through a VisualElement, it might be a LBSTile, QuestNodeView, or even different
         * classes from the same assistant. Eventually, all VisualElements are stored in MainView
         * dictionaries, using the defined object as key.
         
         * It is important that the key is representative of the visualElement, since if you want to
         * access it to update it, or delete it, you are gonna need to get that key's reference. For
         * example, in a tile map, every tile is representative of the element drawn on its position.
        
         * VisualElements are added and erased from the MainView as the following cycle:
         * Note that "tile" here is used as any VisualElement that represents an object.
         *
         * 1.- The expired tiles are removed.
         *     MainView gets the _expiredTiles set from all the assistant in a layer, while clearing
         *     their memory, and erases them if they are in the view.
         *
         * 2.- The Drawers create new tiles.
         *      2.1.- Each Drawer gets the new elements from their respective assistants, while clearing
         *            their memory.
         *      2.2.- The new tiles are created from the retrieved elements, and drawn in the
         *            MainView.
         * 
         * 3.- New tiles, and new expired tiles are saved.
         *     In each assistant, when an action is supposed to create a new VisualElement, a representative
         *     object is stored for later in the _newTiles set.
         *     If an action is supposed to make a VisualElement disappear, then the class used for its
         *     construction is again stored for later but in the _expiredTiles set.
        */
        
        // These methods are a safer way of adding new objects to the sets
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
        
        #endregion
    }
}
