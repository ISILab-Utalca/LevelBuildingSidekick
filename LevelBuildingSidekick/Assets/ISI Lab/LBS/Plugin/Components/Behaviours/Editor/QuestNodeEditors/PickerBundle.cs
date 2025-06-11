


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
    public partial class PickerBundle : VisualElement
    {
        private ObjectField objectFieldBundle;
        private Vector2IntField vector2FieldPosition;
        private Button buttonPickerTarget;
        private Label labelLayer;
        
        public Action _onClicked;
        public Action<Bundle> _onBundleChanged;


        #region CONSTRUCTORS
        public PickerBundle() : base()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetBundle");
            if (visualTree == null)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            vector2FieldPosition = this.Q<Vector2IntField>("TargetPosition");
            vector2FieldPosition.tooltip = "Target position in graph.";
            vector2FieldPosition.SetEnabled(false);
            
            objectFieldBundle = this.Q<ObjectField>("TargetFieldBundle");
            if (objectFieldBundle == null)
            {
                Debug.LogError("TargetFieldBundle not found in VisualElement_QuestTargetBundle.uxml");
            }
            else
            {
                objectFieldBundle.SetEnabled(false);
                objectFieldBundle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue is Bundle bundle) _onBundleChanged?.Invoke(bundle);
                });
            }

            buttonPickerTarget = this.Q<Button>("PickerTarget");
            if (buttonPickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetBundle.uxml");
                return;
            }

            buttonPickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                _onClicked?.Invoke();
            };
            
            labelLayer = this.Q<Label>("Layer");
   
        }

        #endregion
        
        #region METHODS
        
       /// <summary>
       /// Call only during init of editor
       /// </summary>
       /// <param name="label">Description of target</param>
       /// <param name="graphOnly">whether the target must be assigned from the graph. if TRUE use position on SetTarget! ELSE ignore position.</param>
       public void SetInfo(string label, string tooltip, bool graphOnly = false)
       {
           string suffix = graphOnly ? " (In Graph)" : " (Type)";
           // no need to display if we are selecting types!
           vector2FieldPosition.style.display = graphOnly ? DisplayStyle.Flex : DisplayStyle.None;
           
           objectFieldBundle.labelElement.text = label + suffix;
           objectFieldBundle.SetEnabled(!graphOnly);
           this.tooltip = tooltip;
       }


        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="guid">The guid that belongs to the bundle that gets passed into the ObjectField</param>
        /// <param name="position">The position Assigned to the Vector2Int. Ignore if on SetInfo -graphOnly- was set to false</param>
        public void SetTarget(LBSLayer layer = null, string guid = "" , Vector2Int position = default)
        {
            if (guid != string.Empty)
            {
                var bundle = LBSAssetMacro.LoadAssetByGuid<Bundle>(guid);
                objectFieldBundle.value = bundle;
            }

            if (layer != null)  labelLayer.text = layer.Name;
            labelLayer.style.display = layer == null ? DisplayStyle.None : DisplayStyle.Flex;
            
            vector2FieldPosition.style.display = position == default ? DisplayStyle.None : DisplayStyle.Flex;
            vector2FieldPosition.value = position;
        }

        public void ClearPicker()
        {
            _onClicked = null;
        }
        
        #endregion

    }
}