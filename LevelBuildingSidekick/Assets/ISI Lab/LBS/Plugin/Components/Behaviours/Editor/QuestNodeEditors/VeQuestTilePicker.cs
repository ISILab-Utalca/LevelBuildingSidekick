


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

        public Action _onClicked;
        public Action<Bundle> _onBundleChanged;
        
        #region CONSTRUCTORS
        public VeQuestTilePicker() : base()
        {
        }
        #endregion
        
        #region METHODS
        protected VisualElement CreateVisualElement()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetBundle");
            visualTree.CloneTree(this);
            
            TargetBundle = this.Q<ObjectField>("TargetFieldBundle");
            TargetBundle.SetEnabled(false);
            TargetBundle.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is Bundle bundle) _onBundleChanged.Invoke(bundle);
            });
            
            PickerTarget = this.Q<Button>("PickerTarget");
            return this;
        }

       /// <summary>
       /// Call only during init of editor
       /// </summary>
       /// <param name="Label">Description of target</param>
       /// <param name="GraphOnly">whether the target must be assigned from the graph</param>
        public void SetInfo(string Label, bool GraphOnly)
        {
            TargetBundle.labelElement.text = Label;
            TargetBundle.SetEnabled(!GraphOnly);
        }

        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="guid"></param>
        public void SetTarget(ref string guid)
        {
            PickerTarget.clicked += () => _onClicked.Invoke();
        }
        
        #endregion
        
        /*   PickerLocation = this.Q<Button>("PickerLocation");
            PickerLocation.clicked += () => {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                qp.activeData = behaviour.SelectedQuestNode.NodeData;
            };
        */
    }
}