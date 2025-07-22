using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    [System.Serializable]
    [RequieredModule(typeof(PathOSModule))]
    public class PathOSBehaviour : LBSBehaviour
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired] // GABO TODO: Deberia ir o no JsonIgnore?????? De todas maneras hay error de serialziacion (queda nulo)
        private PathOSModule module;
        #endregion

        #region META-FIELDS
        [JsonIgnore]
        public Bundle selectedToSet;
        #endregion

        #region PROPERTIES
        public List<PathOSTile> Tiles { get { return module.GetTiles(); } private set { } }
        #endregion

        #region EVENTS
        public event Action<PathOSModule, PathOSTile> OnAddTile
        {
            add { module.OnAddTile += value; }
            remove { module.OnAddTile -= value; }
        }
        public event Action<PathOSModule, PathOSTile> OnApplyEventTile
        {
            add { module.OnApplyEventTile += value; }
            remove { module.OnApplyEventTile -= value; }
        }
        public event Action<PathOSModule, PathOSTile> OnRemoveTile
        {
            add { module.OnRemoveTile += value; }
            remove { module.OnRemoveTile -= value; }
        }
        #endregion

        #region CONSTRUCTORS
        public PathOSBehaviour(VectorImage icon, string name) : base(icon, name, Color.black) { }
        #endregion

        #region METHODS
        public void AddTile(PathOSTag tag, int x, int y)
        {
            var tile = new PathOSTile(this, x, y, tag);

            // Add Tile or ApplyEventTile segun defina el tag asociado
            // Tags de Elementos
            if (tag.Category == PathOSTag.PathOSCategory.ElementTag)
            {
                // Si el tile a agregar es del AGENTE, se restringe a uno:
                // Si ya existe, se borra el anterior.
                if (tag.Label == "PathOSAgent")
                {
                    var oldAgentTile = module.GetTiles().Find(t => t.Tag.Label == "PathOSAgent");
                    if (oldAgentTile != null)
                    {
                        module.RemoveTile(oldAgentTile);
                    }
                }
                module.AddTile(tile);
            }
            // Tags de eventos
            else if (tag.Category == PathOSTag.PathOSCategory.EventTag)
            {
                PathOSTile oldTile = module.GetTile(x, y);
                // El tile de agente no puede recibir eventos
                if (oldTile != null && oldTile.Tag.Label == "PathOSAgent") { return; }
                // Un tile de muro no puede recibir tags de Trigger
                if (oldTile != null &&
                    oldTile.Tag.Label == "Wall" &&
                    (tag.Label == "DynamicObstacleTrigger" || tag.Label == "DynamicTagTrigger")) { return; }

                module.ApplyEventTile(tile);
            }
        }

        public void RemoveTile(int x, int y)
        {
            var t = module.GetTile(x, y);
            module.RemoveTile(t);
        }

        public PathOSTile GetTile(int x, int y)
        {
            return module.GetTile(x, y);
        }

        public override object Clone()
        {
            return new PathOSBehaviour(this.Icon, this.Name);
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
            module = OwnerLayer.GetModule<PathOSModule>();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public override void OnGUI()
        {
            
        }

        //public override bool Equals(object obj)
        //{
        //    var other = obj as PathOSBehaviour;

        //    if (other == null) return false;

        //    return true;
        //}

        //public override int GetHashCode()
        //{
        //    return base.GetHashCode();
        //}
        #endregion
    }
}
