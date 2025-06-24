using System;
using ISILab.Commons.Utility.Editor;
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
                OnClicked?.Invoke();
            };

            _labelLayer = this.Q<Label>("Layer");
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

            this.tooltip = paramTooltip;
        }

        /// <summary>
        /// Call this when the active node is changed to update the UI.
        /// </summary>
        /// <param name="layer">Optional: the layer to display.</param>
        /// <param name="guid">Optional: the bundle GUID to load and show.</param>
        /// <param name="position">Optional: position in the graph to show.</param>
        public void SetTarget(string layerID = null, string guid = "", Vector2Int position = default)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                var bundle = LBSAssetMacro.LoadAssetByGuid<Bundle>(guid);
                _objectFieldBundle.value = bundle;
            }

            if (layerID != null)
            {
                _labelLayer.text = layerID;
                _labelLayer.style.display = DisplayStyle.Flex;
            }
            else
            {
                _labelLayer.style.display = DisplayStyle.None;
            }

            bool hasPosition = position != default;
            _vector2FieldPosition.style.display = hasPosition ? DisplayStyle.Flex : DisplayStyle.None;
            _vector2FieldPosition.value = position;
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
