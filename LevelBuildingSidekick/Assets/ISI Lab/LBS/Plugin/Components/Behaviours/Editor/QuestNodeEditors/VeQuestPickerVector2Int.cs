


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
    public partial class VeQuestPickerVector2Int : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<VeQuestPickerVector2Int, UxmlTraits> { }
        
        private Vector2IntField TargetPosition;
        private Button PickerTarget;

        public Action _onClicked;


        #region CONSTRUCTORS
        public VeQuestPickerVector2Int() : base()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetPosition");
            if (visualTree == null)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            TargetPosition = this.Q<Vector2IntField>("TargetPosition");
            TargetPosition.tooltip = "The position that must be reached in the graph.";
            TargetPosition.SetEnabled(true);
            
            PickerTarget = this.Q<Button>("PickerTarget");
            if (PickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetPosition.uxml");
                return;
            }

            PickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                _onClicked?.Invoke();
            };
            
            Debug.Log("VeQuestTilePicker Created!");
        }

        #endregion
        
        #region METHODS
        
       /// <summary>
       /// Call only during init of editor
       /// </summary>
       /// <param name="label">Description of target</param>
       /// <param name="graphOnly">whether the target must be assigned from the graph.</param>
       public void SetInfo(string label, string tooltip)
       {
           TargetPosition.labelElement.text = label;
           this.tooltip = tooltip;
       }


        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="guid"></param>
        public void SetTarget(Vector2Int position = default)
        {
            TargetPosition.value = position;
        }

        public void ClearPicker()
        {
            _onClicked = null;
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