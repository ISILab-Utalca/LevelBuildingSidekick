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
            foreach(LBSLayer layer in layers)
            {
                if(layer.ID == layerReference.ID)
                {
                    switch (layerReference.ID)
                    {
                        case "Population":
                            break;
                        case "Exterior":
                            break;
                        case "Interior":
                            break;
                        case "Quest":
                            break;
                    }
                }
            }
            
        }
    }
}