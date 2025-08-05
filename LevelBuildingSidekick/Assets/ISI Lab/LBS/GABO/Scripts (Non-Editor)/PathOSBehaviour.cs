using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using PathOS;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Bundle> pathOSBundles;
        #endregion

        #region PROPERTIES
        public List<PathOSTile> Tiles { get { return module.GetTiles(); } private set { } }

        public List<Bundle> Bundles
        {
            get
            {
                if(pathOSBundles == null || pathOSBundles.Count == 0)
                    pathOSBundles = LBSAssetsStorage.Instance.Get<Bundle>()
                        .Where(b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>().Count > 0).ToList();
                return pathOSBundles;
            }
        }
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
        public PathOSBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint) { }
        #endregion

        #region METHODS
        public void AddTile(LBSTag tag, int x, int y)
        {
            var tile = new PathOSTile(this, x, y, tag);

            bool isElement = true;
            bool isEvent = false;

            // Add Tile or ApplyEventTile segun defina el tag asociado
            // Tags de Elementos
            //if (tag.Category == PathOSTag.PathOSCategory.ElementTag)
            if(isElement)
            {
                // Si el tile a agregar es del AGENTE, se restringe a uno:
                // Si ya existe, se borra el anterior.
                //if (tag.Label == "PathOSAgent")
                //{
                //    var oldAgentTile = module.GetTiles().Find(t => t.Tag.Label == "PathOSAgent");
                //    if (oldAgentTile != null)
                //    {
                //        module.RemoveTile(oldAgentTile);
                //        RequestTileRemove(oldAgentTile);
                //    }
                //}

                PathOSTile old = module.GetTile(tile.X, tile.Y);
                //PathOSTile old = module.GetTile(tile);
                if (old != null) 
                {
                    RequestTileRemove(old);
                }
                module.AddTile(tile);
                RequestTilePaint(tile);
            }
            // Tags de eventos
            //else if (tag.Category == PathOSTag.PathOSCategory.EventTag)
            else if(isEvent)
            {
                PathOSTile oldTile = module.GetTile(x, y);
                // El tile de agente no puede recibir eventos
                if (oldTile != null && oldTile.Tag.Label == "PathOSAgent") { return; }
                // Un tile de muro no puede recibir tags de Trigger
                if (oldTile != null &&
                    oldTile.Tag.Label == "Wall" &&
                    (tag.Label == "DynamicObstacleTrigger" || tag.Label == "DynamicTagTrigger")) { return; }

                if (module.ApplyEventTile(tile))
                {
                    // I know this looks weird but it works like this
                    RequestTileRemove(oldTile);
                    RequestTilePaint(tile);
                    RequestTilePaint(oldTile);
                }
            }
        }

        public void RemoveTile(int x, int y)
        {
            var t = module.GetTile(x, y);
            module.RemoveTile(t);
            RequestTileRemove(t);
        }

        public PathOSTile GetTile(int x, int y)
        {
            return module.GetTile(x, y);
        }

        public void MapToPopulation(List<TileBundleGroup> groups)
        {
            //string s = string.Empty;
            //foreach(KeyValuePair<EntityType, PathOSStorage.SimulationEntityData> pair in PathOSStorage.Instance.entityDataPool)
            //{
            //    s += "Entity Type: " + pair.Key + " | Texture: " + (pair.Value.image ? pair.Value.image.name : null) + "\n";
            //}
            //Debug.Log(s);
            foreach(TileBundleGroup group in groups)
            {
                BundleData bundle = group.BundleData;
                var characteristics = bundle.Bundle.GetCharacteristics<LBSTagsCharacteristic>();
                if(characteristics.Count == 0)
                {
                    Debug.LogWarning($"Bundle '{bundle.BundleName}' doesn't have any LBSTagsCharacteristic.");
                    continue;
                }
                LBSTag tag = null;
                bool validTag = false;
                //bool playerTag = false;
                for(int i = 0; i < characteristics.Count; i++)
                {
                    tag = characteristics[i].Value;
                    if(tag != null)
                    {
                        if(tag.EntityType != EntityType.ET_NONE)
                        {
                            validTag = true;
                            break;
                        }
                        else if(tag.Label.Equals("Player"))
                        {
                            //var pathOSTags = Bundles.Select(b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0]).ToList();
                            //var agentTag = pathOSTags//pathOSBundles.Select(b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0])
                            //    .FirstOrDefault(tag => tag.Value.Label.Equals("PathOSAgent"));
                            //tag = agentTag//pathOSBundles.Select(b => b.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0])
                            //    .Value.ToLBSTag();
                            
                            validTag = true;
                            break;
                        }
                    }
                }
                //if (playerTag) // Deberia crear un agente
                //    continue;
                if(!validTag)
                {
                    Debug.LogWarning($"Bundle '{bundle.BundleName}' tags are invalid. Check for null values at LBSTagsCharacteristic or the DefaultType property of your LBSTags assets.");
                    continue;
                }
                foreach(LBSTile tile in group.TileGroup)
                {
                    Vector2Int pos = tile.Position;
                    AddTile(tag, pos.x, pos.y);
                }
            }
        }

        public void ClearMapping()
        {
            throw new NotImplementedException("This feature is currently not available.");
        }

        public override object Clone()
        {
            return new PathOSBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
            module = OwnerLayer.GetModule<PathOSModule>();
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            OwnerLayer = null;
        }

        public override void OnGUI()
        {
            
        }

        public override bool Equals(object obj)
        {
            var other = obj as PathOSBehaviour;

            if (other == null) return false;

            if(!Equals(Name, other.Name)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
