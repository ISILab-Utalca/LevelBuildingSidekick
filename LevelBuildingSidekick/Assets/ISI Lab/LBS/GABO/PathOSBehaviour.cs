using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    [System.Serializable]
    [RequieredModule(typeof(PathOSModule))]
    public class PathOSBehaviour : LBSBehaviour
    {
        #region ENUMS(sin usar por ahora)
        //public enum EntityType
        //{
        //    ET_NONE = 0,
        //    ET_GOAL_OPTIONAL = 100,
        //    ET_GOAL_MANDATORY = 110,
        //    ET_GOAL_COMPLETION = 120,
        //    ET_RESOURCE_ACHIEVEMENT = 150,
        //    ET_RESOURCE_PRESERVATION_LOW = 160,
        //    ET_RESOURCE_PRESERVATION_MED = 170,
        //    ET_RESOURCE_PRESERVATION_HIGH = 180,
        //    ET_HAZARD_ENEMY_LOW = 200,
        //    ET_HAZARD_ENEMY_MED = 210,
        //    ET_HAZARD_ENEMY_HIGH = 220,
        //    ET_HAZARD_ENEMY_BOSS = 230,
        //    ET_HAZARD_ENVIRONMENT = 250,
        //    ET_POI = 300,
        //    ET_POI_NPC = 350
        //};
        #endregion

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
        public PathOSBehaviour(Texture2D icon, string name) : base(icon, name) { }
        #endregion

        #region METHODS
        public void AddTile(PathOSTag tag, int x, int y)
        {
            var tile = new PathOSTile(this, x, y, tag);

            // Add Tile or ApplyEventTile segun defina el tag asociado
            // Tags de Elementos
            if (tag.Category == PathOSTag.PathOSCategory.ElementTag)
            {
                // Si el tile a agregar es el del agente, se restringe a uno:
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
                // El tile de agente no puede recibir eventos
                if (module.GetTile(x, y).Tag.Label == "PathOSAgent") { return; }

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
            Owner = layer;
            module = Owner.GetModule<PathOSModule>();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
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
