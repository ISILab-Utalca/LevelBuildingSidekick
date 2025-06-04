


// ReSharper disable All

using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using LBS;
using LBS.Bundles;
using LBS.Components;
using LBS.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    public class VeQuestTilePicker : VisualElement
    {
        private ObjectField TargetBundle;
        private Button PickerTarget;

        private Action<LBSLayer> _onClicked;
        
        #region CONSTRUCTORS
        public VeQuestTilePicker() : base()
        {
        }
        #endregion
        
        #region METHODS
        protected override VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetBundle");
            visualTree.CloneTree(this);
            
            TargetBundle = this.Q<ObjectField>("TargetFieldBundle");
            TargetBundle.SetEnabled(false);
            TargetBundle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is Bundle bundle) SetTargetValue(bundle);
            });
            
            PickerTarget = this.Q<Button>("PickerTarget");
            PickerTarget.clicked += () =>
            {
                
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                qp.activeData = behaviour.SelectedQuestNode.NodeData;
                qp.OnBundlePicked = (string guid) =>
                {
                    ///behaviour.SelectedQuestNode.NodeData.bundleGuid = guid;
                };
            };
            return this;
        }

        /// <summary>
        /// Some missions like kill require that that the bundles
        /// are picked from the existing graph
        /// </summary>
        /// <param name="value"></param>
        public void MustSelectGraph(bool value)
        {
            TargetBundle.SetEnabled(!value);
        }
        
    }
}