


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
    public partial class PickerVector2Int : VisualElement
    {
        public Vector2IntField vector2IntField;
        private Button pickerTarget;

        public Action _onClicked;

        #region CONSTRUCTORS
        public PickerVector2Int() : base()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetPosition");
            if (visualTree == null)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            vector2IntField = this.Q<Vector2IntField>("TargetPosition");
            vector2IntField.tooltip = "The position that must be reached in the graph.";
            vector2IntField.SetEnabled(true);
            
            pickerTarget = this.Q<Button>("PickerTarget");
            if (pickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetPosition.uxml");
                return;
            }

            pickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                var qp = ToolKit.Instance.GetActiveManipulatorInstance() as QuestPicker;
                _onClicked?.Invoke();
            };
            
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
           vector2IntField.labelElement.text = label;
           this.tooltip = tooltip;
       }


        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="guid"></param>
        public void SetTarget(Vector2Int position = default)
        {
            vector2IntField.value = position;
        }

        public void ClearPicker()
        {
            _onClicked = null;
        }


        public void DisplayVectorField(bool display)
        {
            vector2IntField.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        #endregion

    }
}