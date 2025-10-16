using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using ISILab.LBS.CustomComponents;
using ISILab.Macros;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class LayerView : VisualElement
    {
        #region FIELDS
        private LBSLayer _target;

        private TextField _layerName;
        private VisualElement _layerIcon;
        private VisualElement _iconsModules;
        private VisualElement _iconFocus;
        private Button _showButton;
        private Button _hideButton;

        public VisualElement Base;

        private Action _onVisibilityChange;
        private Action _onNameChange;
        #endregion

        #region PROPERTIES
        public LBSLayer Target { get => _target; set => _target = value; }
        #endregion
        
        #region EVENTS
        public event Action OnVisibilityChange
        {
            add => _onVisibilityChange += value;
            remove => _onVisibilityChange -= value;
        }
        public event Action OnNameChange
        {
            add => _onNameChange += value;
            remove => _onNameChange -= value;
        }

        public Action OnLayerVisibilityChangeAction;
        #endregion

        #region CONSTRUCTOR
        public LayerView()
        {
            CloneVisualTree();
            BindUIElements();
            Callbacks();
            SetStyleSelectors();
            
            


        }


        #endregion

        #region METHODS

        #region INITIALIZATION
        private void CloneVisualTree()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayerView");
            visualTree.CloneTree(this);
            Base = this;
            Base.name = "Base";
        }

        private void BindUIElements()
        {
            _layerName = this.Q<TextField>("Name");
            _layerIcon = this.Q<VisualElement>("Icon");
            _iconFocus = this.Q<VisualElement>("IconFocus");
            _iconsModules = this.Q<VisualElement>("IconsModules");
            _showButton = this.Q<Button>("ShowButton");
            _hideButton = this.Q<Button>("HideButton");

            _iconFocus.style.display = DisplayStyle.None;
        }

        private void Callbacks()
        {
            _layerName.RegisterCallback<ChangeEvent<string>>(OnNameChanged);
            _showButton.clicked += () => ShowLayer(true);
            _hideButton.clicked += () => ShowLayer(false);
        }
        
        public void SetStyleSelectors()
        {
            RemoveFromClassList("unity-collection-view__item");
            RemoveFromClassList("unity-list-view__item");
            RemoveFromClassList("unity-collection-view__item:selected");
            RemoveFromClassList("unity-collection-view__item:hover");
            
            AddToClassList("lbs-list-item");
        }
        #endregion

        #region INFO & UI UPDATE
        public void SetInfo(LBSLayer layer)
        {
            _target = layer;
            SetIcon(layer.iconGuid);
            SetName(layer.Name);

            layer.OnAddModule += (_, _) => ShowModulesIcons();
            ShowLayer(layer.IsVisible);
        }

        private void SetName(string newName) => _layerName.value = newName;

        private void SetIcon(string guid)
        {
            var icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(guid);
            _layerIcon.style.backgroundImage = new StyleBackground(icon);
        }

        private void ShowModulesIcons()
        {
            _iconsModules.Clear();
            foreach (var module in _target.Modules)
            {
                var icon = new VisualElement();
                icon.style.height = icon.style.width = 16;
                _iconsModules.Add(icon);
            }
        }

        private void ShowLayer(bool value)
        {
            _showButton.style.display = value ? DisplayStyle.None : DisplayStyle.Flex;
            _hideButton.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;

            if(_target.IsVisible != value)
            {
                _target.IsVisible = value;
                _onVisibilityChange?.Invoke();
            }
        }
        #endregion

        #region CALLBACKS
        private void OnNameChanged(ChangeEvent<string> evt)
        {
            _target.Name = evt.newValue;
            _onNameChange?.Invoke();
        }
        #endregion

        #region SELECTION
        public void UpdateSelect(LBSLayer layer, bool FocusToggle = false)
        {
            _iconFocus.style.display = DisplayStyle.None;
    
            if (layer is null || !layer.Equals(_target))
            {
                //RemoveFromClassList("lbs-list-item:selected");
                return;
            }
            _iconFocus.style.display = FocusToggle ? DisplayStyle.Flex :  DisplayStyle.None;
            AddToClassList("unity-collection-view__item:selected");
        }


        
        
        
        #endregion

        #endregion
    }
}
