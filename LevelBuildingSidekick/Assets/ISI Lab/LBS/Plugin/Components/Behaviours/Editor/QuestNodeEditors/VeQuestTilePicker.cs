


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
    [UxmlElement]
    public partial class VeQuestTilePicker : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<VeQuestTilePicker, UxmlTraits> { }
        
        private ObjectField TargetBundle;
        private Vector2IntField TargetPosition;
        private Button PickerTarget;

        public Action _onClicked;
        public Action<Bundle> _onBundleChanged;


        #region CONSTRUCTORS
        public VeQuestTilePicker() : base()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetBundle");
            if (visualTree == null)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            TargetPosition = this.Q<Vector2IntField>("TargetPosition");
            TargetPosition.tooltip = "Target position in graph.";
            TargetPosition.SetEnabled(false);
            
            TargetBundle = this.Q<ObjectField>("TargetFieldBundle");
            if (TargetBundle == null)
            {
                Debug.LogError("TargetFieldBundle not found in VisualElement_QuestTargetBundle.uxml");
            }
            else
            {
                TargetBundle.SetEnabled(false);
                TargetBundle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue is Bundle bundle) _onBundleChanged?.Invoke(bundle);
                });
            }

            PickerTarget = this.Q<Button>("PickerTarget");
            if (PickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetBundle.uxml");
                return;
            }

            PickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                _onClicked.Invoke();
            };
            
            Debug.Log("VeQuestTilePicker Created!");
        }

        #endregion
        
        #region METHODS
        
       /// <summary>
       /// Call only during init of editor
       /// </summary>
       /// <param name="label">Description of target</param>
       /// <param name="graphOnly">whether the target must be assigned from the graph</param>
       public void SetInfo(string label, string tooltip, bool graphOnly = false)
       {
           string suffix = graphOnly ? " (In Graph)" : " (Type)";
           TargetBundle.labelElement.text = label + suffix;
           TargetBundle.SetEnabled(!graphOnly);
           this.tooltip = tooltip;
       }


        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="guid"></param>
        public void SetTarget(string guid, Vector2Int position = default)
        {
            var bundle = LBSAssetMacro.LoadAssetByGuid<Bundle>(guid);
            TargetBundle.value = bundle;
            
            TargetPosition.style.display = position == default ? DisplayStyle.None : DisplayStyle.Flex;
            TargetPosition.value = position;
        }

        public void ClearPicker()
        {
            _onClicked = null;
            _onBundleChanged = null;
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