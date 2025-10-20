


// ReSharper disable All

using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Settings;
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
    public partial class PickerVector2Int : PickerBase
    {
        private Color _color = LBSSettings.Instance.view.toolkitNormal;
        private Color _selected = LBSSettings.Instance.view.newToolkitSelected;
        
        public Vector2IntField vector2IntField;
        private Button _buttonPickerTarget;

        public Action _onClicked;

        #region CONSTRUCTORS
        public PickerVector2Int() : base()
        {
            Clear();
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PickerVector2Int");
            if (visualTree == null)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            vector2IntField = this.Q<Vector2IntField>("TargetPosition");
            vector2IntField.tooltip = "The position that must be reached in the graph.";
            vector2IntField.SetEnabled(true);
            
            _buttonPickerTarget = this.Q<Button>("PickerTarget");
            if (_buttonPickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetPosition.uxml");
                return;
            }

            _buttonPickerTarget.clicked += () =>
            {
                ActivateButton(_buttonPickerTarget);
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
       /// <param name="tooltip">Description of what this position represents.</param>
       public void SetInfo(string label, string tooltip)
       {
           vector2IntField.labelElement.text = label;
           this.tooltip = tooltip;
       }


        /// <summary>
        /// Call whenever a active node is changed
        /// </summary>
        /// <param name="position">a position to assign into the field</param>
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