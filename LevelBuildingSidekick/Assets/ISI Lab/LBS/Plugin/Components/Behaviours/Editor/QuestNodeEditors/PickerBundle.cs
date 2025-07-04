using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using LBS.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class PickerBundle : VisualElement
    {
        private readonly ObjectField _objectFieldBundle;
        private readonly Vector2IntField _vector2FieldPosition;
        private readonly Button _buttonPickerTarget;
        private readonly Label _labelLayer;

        public Action OnClicked;
        private readonly Action<Bundle> _onBundleChanged = null;

        #region Constructors

        public PickerBundle()
        {
            Clear();

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("VisualElement_QuestTargetBundle");
            if (!visualTree)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

            _vector2FieldPosition = this.Q<Vector2IntField>("TargetPosition");
            _vector2FieldPosition.tooltip = "Target position in graph.";
            _vector2FieldPosition.SetEnabled(false);

            _objectFieldBundle = this.Q<ObjectField>("TargetFieldBundle");
            if (_objectFieldBundle == null)
            {
                Debug.LogError("TargetFieldBundle not found in VisualElement_QuestTargetBundle.uxml");
            }
            else
            {
                _objectFieldBundle.SetEnabled(false);
                _objectFieldBundle.RegisterValueChangedCallback(BundleChangeCallback);
            }

            _buttonPickerTarget = this.Q<Button>("PickerTarget");
            if (_buttonPickerTarget == null)
            {
                Debug.LogError("PickerTarget not found in VisualElement_QuestTargetBundle.uxml");
                return;
            }

            _buttonPickerTarget.clicked += () =>
            {
                ToolKit.Instance.SetActive(typeof(QuestPicker));
                
                // by default not picking the main trigger - its set on its OnClicked Implementation on QuestNodeBehaviourEditor
                var mani = ToolKit.Instance.GetActiveManipulator();
                if (mani is QuestPicker questPicker) questPicker.PickTriggerPosition = false;
                
                OnClicked?.Invoke();
            };

            _labelLayer = this.Q<Label>("Layer");

            DefaultValues();
        }

        private void BundleChangeCallback(ChangeEvent<Object> evt)
        {
            if (evt.newValue is Bundle bundle) _onBundleChanged?.Invoke(bundle);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Should be called during editor initialization.
        /// </summary>
        /// <param name="paramLabel">Description of the target.</param>
        /// <param name="paramTooltip">Tooltip shown on hover.</param>
        /// <param name="graphOnly">If true, disables direct bundle selection and only allows selection from graph.</param>
        public void SetInfo(string paramLabel, string paramTooltip, bool graphOnly = false)
        {
            string suffix = graphOnly ? " (In Graph)" : " (Type)";
            _vector2FieldPosition.style.display = graphOnly ? DisplayStyle.Flex : DisplayStyle.None;

            _objectFieldBundle.labelElement.text = paramLabel + suffix;
            _objectFieldBundle.SetEnabled(!graphOnly);

            tooltip = paramTooltip;
        }

        /// <summary>
        /// Call this when the active node is changed to update the UI.
        /// </summary>
        /// <param name="layerTarget"></param>
        public void SetEditorLayerTarget(LayerTarget layerTarget)
        {
            if (layerTarget == null) return;
            var guid = layerTarget.GetGuid();
            if (!string.IsNullOrEmpty(guid))
            {
                var bundle = LBSAssetMacro.LoadAssetByGuid<Bundle>(guid);
                _objectFieldBundle.value = bundle;
            }
            else
            {
                DefaultValues();
            }

            _vector2FieldPosition.style.display = DisplayStyle.None;
            if (layerTarget is not BundleGraph bg) return;
            
            
            var layer = layerTarget.GetLayer();
            if(layer != null)
            {
                layer.OnChangeName += () =>
                {
                    _labelLayer.text = layer.Name;
                };
                
                _labelLayer.text = layer.Name;
                _labelLayer.style.display = DisplayStyle.Flex;
            }
            else
            {
                _labelLayer.style.display = DisplayStyle.None;
            }

            _vector2FieldPosition.style.display = DisplayStyle.Flex;
            _vector2FieldPosition.value = bg.Position;

        }

        private void DefaultValues()
        {
            _objectFieldBundle.value = null;
            _labelLayer.text = "No Layer";
            _vector2FieldPosition.value = Vector2Int.zero;
        }


        /// <summary>
        /// Clears the picker click callback.
        /// </summary>
        public void ClearPicker()
        {
            OnClicked = null;
        }

        #endregion

        
    }
}
