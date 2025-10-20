using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Components
{
    
    
    #region Targets
    [Serializable]
    public abstract class LayerTarget
    {
        [SerializeReference][SerializeField]protected LBSLayer layer;
        
        public LBSLayer GetLayer() => layer;
        
        public string GetLayerName()
        {
            return layer?.Name ?? "";
        }

        public abstract string GetGuid();
        public abstract bool Valid();
        
        public override bool Equals(object obj)
        {
            var other =  obj as LayerTarget;
            return other?.GetGuid() == GetGuid();
        }

    }
    
    /// <summary>
    /// Saves the bundle guid and the position in the graph to get in the scene
    /// </summary>
    /// 
    [Serializable]
    public class BundleGraph : LayerTarget
    {
        [SerializeReference] [SerializeField] private TileBundleGroup tileBundle;
        [SerializeField] private string guid;
        [SerializeField] private BaseQuestNodeData _nodeData;
        // must be assigned on all bundleGraphs to the Resize Function
        
        public BundleGraph(BaseQuestNodeData nodeData, LBSLayer layer = null, TileBundleGroup tileBundle = null)
        {
            this.layer = layer;
            
            this.tileBundle = tileBundle;
            _nodeData = nodeData;
            
            if(this.tileBundle is null) return;
            guid = this.tileBundle.GetGuid();
            
            if(_nodeData is null) return;
            this.tileBundle!.OnRemoved += ClearTileBundle;
        }

        private void ClearTileBundle()
        {
            tileBundle = null;
        }

        public Vector2Int Position => new((int)Area.x, (int)Area.y);
        public Rect Area
        {
            get
            {   
                if(tileBundle is null) return Rect.zero;
                return tileBundle.AreaRect;
            }
        }

        public override bool Valid() => GetGuid() != string.Empty;
        public override string GetGuid()
        {
            return guid;
        }
        
    }
    
    
    /// <summary>
    /// Saves the bundle type
    /// </summary>
    [Serializable]
    public class BundleType : LayerTarget
    {
        [SerializeField]private string guid;
     
        // TODO clean up this class
        public BundleType(
            LBSLayer layer = null, 
            TileBundleGroup tileBundle = null)
        {
            this.layer = layer;
            if (tileBundle != null) guid = tileBundle.GetGuid();
        }

        public override string GetGuid()
        {
            return guid;
        }

        public override bool Valid()
        {
            return GetGuid()!= string.Empty;
        }
        
    }
    
    #endregion
    
    /// <summary>
    /// <para><b>FOR LBS USER</b></para>
    /// 
    /// <para>
    /// This class is meant to store data for terminal actions<c>string</c> declared in an
    /// Scriptable Object<c>LBSGrammar</c>.
    /// If the grammar is modified (actions) or a new grammar is assigned, 
    /// new children of this class must be created.
    /// </para>
    /// 
    /// <para>
    /// To assign data, create your own VisualElement child of <c>NodeEditor</c>
    /// </para>
    /// 
    /// <para>
    /// Remember to update the <see cref="QuestNodeDataFactory"/> by adding 
    /// an action <c>string</c> and its  
    /// <see cref="BaseQuestNodeData"/> child class.
    /// </para>
    /// </summary>
    [Serializable]
    public abstract class BaseQuestNodeData
    {
        #region FIELDS
        [SerializeField, SerializeReference, JsonRequired]
        protected QuestNode ownerNode;
        
        [SerializeField, JsonRequired]
        protected string tag;

        [SerializeField, JsonRequired] 
        protected Rect area;
        
        [SerializeField, JsonRequired] 
        protected Color color =  LBSSettings.Instance.view.behavioursColor;

        [SerializeField, JsonRequired] 
        protected string iconGuid = LocationIcon;

        // The default trigger icon
        protected static string LocationIcon = "efd5e48bd83c08d469fcc341c886b38b";
        // Use to indicate friendly npc
        protected static string StarIcon = "99b7816ce61fd85449ad2379f39bb8c2";
        // Use to indicate foes
        protected static string FoeIcon = "d0baea4f8bdb0c948887aed23edd4cad";
        // Use to indicate objects or items
        protected static string ObjectIcon = "699cc90614aad8047875eb0fae8b175f";
            
        #endregion

        #region PROPERTIES
        public QuestNode OwnerNode => ownerNode;
        

        public QuestGraph Graph => ownerNode.Graph;
        
        public LBSLayer Layer => Graph.OwnerLayer;
        
        public string Tag => tag;

        public Rect Area
        {
            get => area;
            set => area = value;
        }

        public Color Color => color;
        public string ID => OwnerNode.ID;

        #endregion

        protected BaseQuestNodeData(QuestNode ownerNode, string tag)
        {
            this.ownerNode = ownerNode;
            this.tag = tag;

            if (ownerNode?.Graph?.OwnerLayer == null) return;
  
            var pos = ownerNode.Graph.OwnerLayer.ToFixedPosition(ownerNode.Position);
            area = new Rect(pos.x, pos.y, 1, 1);
        }


        public virtual void Clone(BaseQuestNodeData data)
        {
            ownerNode = data.ownerNode;
            tag = data.tag;
            area = data.area;
        }

        // by default there are no references to other layers.
        public virtual List<string> ReferencedLayerNames()
        {
            return null;
        }

        // by default no resize. Implement if using bundleGraph fields
        public virtual void Resize()
        {
           
        }
        protected void ResizeToFitBundles(IEnumerable<BundleGraph> bundles)
        {
            var validRects = bundles
                .Where(b => (b is not null) && b.Valid())
                .Select(b => b.Area)
                .ToList();

            if (validRects.Count == 0)
                return;

            // Only use the rects' x and y positions
            float minX = validRects.Min(r => r.x);
            float maxX = validRects.Max(r => r.x + r.width);
            float minY = validRects.Min(r => r.y - r.height);
            float maxY = validRects.Max(r => r.y ); // subtract height because of inverted Y in graph

            float width = maxX - minX;
            float height = maxY - minY;

            // Inverted Y origin: anchor from maxY going downward
            area = new Rect(minX, maxY, Mathf.Abs(width), Mathf.Abs(height));
        }

        public abstract bool Equals(BaseQuestNodeData other);
        
        public abstract bool IsValid();

        public VectorImage GetIcon()
        {
            return LBSAssetMacro.LoadAssetByGuid<VectorImage>(iconGuid);
        }

        /// <summary>
        /// Used in the quest assistant, assign data to the node by passing tiles
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="tiles"></param>
        public abstract void SetDataByTiles(List<LBSLayer> layers, List<TileBundleGroup> tiles);
        
        #region Helpers

        protected bool TrySetBundleGraphList(
            List<LBSLayer> layers, List<TileBundleGroup> suggestions,
            ref  List<BundleGraph> listVar,
            HashSet<Bundle.EElementFlag> flags,
            bool bRequireAllTags = false)
        {
            foreach (var suggestionTile in suggestions)
            {
                var GraphData = LBSLayerHelper.GetBundleTileByPosition(suggestionTile.AreaRect.position.ToInt(), layers);
                // safe assign clauses
                if (GraphData is null) continue;
                var bundle = GraphData.Item2.BundleData.Bundle;
                
                bool bTagRequirement;
                if (bRequireAllTags)
                {
                    bTagRequirement = bundle.HasAllFlags(flags);
                    if(!bTagRequirement)Debug.Log($"{bundle}: missing a flag ({flags}). Can't assign as bundle graph");
                }
                else
                {
                    bTagRequirement = bundle.HasAnyFlag(flags);
                    if(!bTagRequirement)Debug.Log($"{bundle}: missing does not contain any of ({flags}). Can't assign as bundle graph");
                }
                if(!bTagRequirement) continue;
                
                var newBG = new BundleGraph(this, GraphData.Item1,  GraphData.Item2);
                listVar.Add(newBG);
            }
            
            return listVar.Any();
        }
        
        protected bool TrySetBundleGraph(
            List<LBSLayer> layers, List<TileBundleGroup> suggestions,
            ref BundleGraph graphVar,
            HashSet<Bundle.EElementFlag> flags,
            bool bRequireAllTags = false)
        {
            foreach (var suggestionTile in suggestions)
            {
                var GraphData = LBSLayerHelper.GetBundleTileByPosition(suggestionTile.AreaRect.position.ToInt(), layers);
                if (GraphData is null) continue;
                var bundle = GraphData.Item2.BundleData.Bundle;

                bool bTagRequirement;
                if (bRequireAllTags)
                {
                    bTagRequirement = bundle.HasAllFlags(flags);
                  //  if(!bTagRequirement)Debug.Log($"{bundle}: missing a flag ({flags}). Can't assign as bundle graph");
                }
                else
                {
                    bTagRequirement = bundle.HasAnyFlag(flags);
                //    if(!bTagRequirement)Debug.Log($"{bundle}: missing does not contain any of ({flags}). Can't assign as bundle graph");
                }
                if(!bTagRequirement) continue;
                
                graphVar = new BundleGraph(this, GraphData.Item1,  GraphData.Item2);
                if (graphVar is not null) return true;
            }

            return false;
        }
            
        protected bool TrySetBundleType(
            List<LBSLayer> layers, List<TileBundleGroup> suggestions,
            ref BundleType typeVar,
            HashSet<Bundle.EElementFlag> flags,
            bool bRequireAllTags = false)
        {
            foreach (var suggestionTile in suggestions)
            {
                var GraphData = LBSLayerHelper.GetBundleTileByPosition(suggestionTile.AreaRect.position.ToInt(), layers);
                if (GraphData is null) continue;
                var bundle = GraphData.Item2.BundleData.Bundle;
                bool bTagRequirement;
                if (bRequireAllTags)
                {
                    bTagRequirement = bundle.HasAllFlags(flags);
                 //   if(!bTagRequirement)Debug.Log($"{bundle}: missing a flag ({flags}). Can't assign as bundle graph");
                }
                else
                {
                    bTagRequirement = bundle.HasAnyFlag(flags);
                  //  if(!bTagRequirement)Debug.Log($"{bundle}: missing does not contain any of ({flags}). Can't assign as bundle graph");
                }
                if(!bTagRequirement) continue;
                
                typeVar = new BundleType(null, suggestionTile);
                if (typeVar is not null) return true;
            }
            return false;
        }
        
        #endregion
    }


  
} 