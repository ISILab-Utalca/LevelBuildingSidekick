using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using LBS.Components;
using ISILab.LBS.Behaviours;
using System.Numerics;
using ISILab.Macros;
using System.Collections.Generic;
using ISILab.LBS.Modules;
using ISILab.LBS.Components;
using LBS.Components.TileMap;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class LayerContextEntry : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region ATTRIBUTES
        string layerName;
        LBSLayer layerReference;

        private VisualElement layerImage;
        private Label layerNameLabel;
        private VisualElement entryWarning;
        private Button removeButton;
        #endregion

        #region FIELDS 
        public string LayerName
        {
            get => layerName;
            set => layerName = value;
        }

        public LBSLayer LayerReference
        {
            get => layerReference;
            set => layerReference = value;
        }
        #endregion

        public Action OnRemoveButtonClicked;

        public LayerContextEntry() {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayerContextEntry");
            visualTree.CloneTree(this);

            layerImage = this.Q<VisualElement>("LayerImage");
            layerNameLabel = this.Q<Label>("LayerName");
            entryWarning = this.Q<VisualElement>("EntryWarning");
            removeButton = this.Q<Button>("RemoveButton");
            removeButton.clicked += () => OnRemoveButtonClicked?.Invoke();

            entryWarning.tooltip = "This layer's information overlaps with another layer of the same type. Remove or modify one of them or the evaluation may not yield the desired results.";
            entryWarning.visible = false;
        }

        public void UpdateData(object layer)
        {
            var layerData = layer as LBSLayer;
            if (layerData == null) return;

            layerReference = layerData;
            name = layerData.Name;
            layerNameLabel.text = layerData.Name;

            SetIcon(layerData.iconGuid);
        }
        private void SetIcon(string guid)
        {
            VectorImage icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(guid);
            layerImage.style.backgroundImage = new StyleBackground(icon);
        }

        public void EvaluateOverlap(List<LBSLayer> layers)
        {
            //Check all layers...
            foreach(LBSLayer layer in layers)
            {
                if (layerReference.Equals(layer)) continue;
                //...But only if they coincide with the ID
                if(layer.ID == layerReference.ID)
                {
                    //Then take action depending on the ID
                    switch (layerReference.ID)
                    {
                        case "Population":
                            var populationBehavior = layer.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
                            var referenceBehaviorPop = layerReference.Behaviours.Find(b => b.GetType().Equals(typeof(PopulationBehaviour))) as PopulationBehaviour;
                            if (populationBehavior == null) continue;
                            if (referenceBehaviorPop == null) continue;

                            //Every group in the reference behavior is checked. If there's a population asset in any location that belongs to the reference behavior, it'll enable the warning.
                            foreach (TileBundleGroup group in referenceBehaviorPop.Tilemap)
                            {
                                var groupBounds = group.GetBounds();
                                for(int i= (int)groupBounds.x; i< (int)groupBounds.x+(int)groupBounds.width; i++)
                                {
                                    for (int j = (int)groupBounds.y; j < (int)groupBounds.y+(int)groupBounds.height; j++)
                                    {
                                        if (populationBehavior.GetTileGroup(new Vector2Int(i, j)) !=null)
                                        {
                                            //Enable warning.
                                            entryWarning.visible = true;
                                            return;
                                        }
                                    }
                                }
                            }
                            break;
                        case "Exterior":
                            //No real way to find discrepancies here for now. There will probably be at some point, though.
                            break;
                        case "Interior":
                            var interiorBehavior = layer.Behaviours.Find(b => b.GetType().Equals(typeof(SchemaBehaviour))) as SchemaBehaviour;
                            var referenceBehaviorInt = layerReference.Behaviours.Find(b => b.GetType().Equals(typeof(SchemaBehaviour))) as SchemaBehaviour;
                            if (interiorBehavior == null) continue;
                            if (referenceBehaviorInt == null) continue;

                            foreach(LBSTile tile in referenceBehaviorInt.Tiles)
                            {
                                if (interiorBehavior.GetTile(tile.Position) != null)
                                {
                                    //Enable warning.
                                    entryWarning.visible = true;
                                    return;
                                }
                            }
                            break;
                        case "Quest":
                            //Nothing to see here lol
                            break;
                    }
                }
            }
            //If it survived the entire switch, it's because there's nothing to worry about and the warning can be disabled.
            entryWarning.visible = false;
        }
    }
}