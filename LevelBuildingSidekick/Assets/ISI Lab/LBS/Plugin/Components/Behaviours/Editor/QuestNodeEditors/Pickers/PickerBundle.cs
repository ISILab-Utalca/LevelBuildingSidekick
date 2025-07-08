using System;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using ISILab.Macros;
using LBS.Bundles;
using LBS.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class PickerBundle : PickerBase
    {
        private readonly ObjectField _objectFieldBundle;
        private readonly Button _buttonPickerTarget;
        private readonly Label _labelLayer;
        private readonly VisualElement _warning;
        private readonly VisualElement _layerDisplay;
        private readonly Vector2IntField _position;
        
        public Action OnClicked;
        private readonly Action<Bundle> _onBundleChanged = null;

        #region Constructors

        public PickerBundle()
        {
            Clear();

            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PickerBundle");
            if (!visualTree)
            {
                Debug.LogError("VisualElement_QuestTargetBundle.uxml not found. Check the file name and folder path.");
                return;
            }

            visualTree.CloneTree(this);

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
                ActivateButton(_buttonPickerTarget);
                OnClicked?.Invoke();
            };

            OnClicked += SetPickerManipulator;

            _warning = this.Q<VisualElement>("Warning");
            _warning.tooltip = "Data Information Missing. Correct nulls and non-assigned values";
            
            
            _labelLayer = this.Q<Label>("Layer");
            _layerDisplay = this.Q<VisualElement>("LayerDisplay");
            _position = this.Q<Vector2IntField>("Position");
            
            
            DefaultValues();
        }

        private static void SetPickerManipulator()
        {
            ToolKit.Instance.SetActive(typeof(QuestPicker));

            // by default not picking the main trigger - its set on its OnClicked Implementation on QuestNodeBehaviourEditor
            var mani = ToolKit.Instance.GetActiveManipulator();
            if (mani is QuestPicker questPicker) questPicker.PickTriggerPosition = false;
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

            _objectFieldBundle.labelElement.text = paramLabel + suffix;
            _objectFieldBundle.SetEnabled(!graphOnly);
            
            _layerDisplay.style.display = graphOnly ? DisplayStyle.Flex : DisplayStyle.None;
            
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
            
            // display danger if invalid
            _warning.style.display = layerTarget.Valid() ? DisplayStyle.None : DisplayStyle.Flex;
            
            _layerDisplay.style.display = DisplayStyle.None;
            if (layerTarget is not BundleGraph bg) return;
            
            
            var layer = layerTarget.GetLayer();
            if(layer != null)
            {
                _layerDisplay.style.display = DisplayStyle.Flex;
                _labelLayer.text = layer.Name;
                _position.value = bg.Position;
                
                layer.OnChangeName += () =>
                {
                    _labelLayer.text = layer.Name;
                    _position.value = bg.Position;
                };
                
            
            }
  
        }

        private void DefaultValues()
        {
            _objectFieldBundle.value = null;
            _labelLayer.text = "No Layer";
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
