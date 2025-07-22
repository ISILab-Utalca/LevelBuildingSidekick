using ISILab.Commons.Utility.Editor;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.Macros;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class LayerView : VisualElement
    {
       // public new class UxmlFactory : UxmlFactory<LayerView, VisualElement.UxmlTraits> { }

        private LBSLayer _target;

        private readonly TextField _layerName;
        private readonly VisualElement _layerIcon;
        private readonly VisualElement _iconsModules;
        private readonly Button _showButton;
        private readonly Button _hideButton;

        public VisualElement Base;

        private Action _onVisibilityChange;
        private Action _onNameChange;
        
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

        public LayerView()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayerView");
            visualTree.CloneTree(this);

            Base = this.Q<VisualElement>("Base");

            // LayerName
            _layerName = this.Q<TextField>("Name");
            _layerName.RegisterCallback<ChangeEvent<string>>(e =>
            {
                _target.Name = e.newValue;
                _onNameChange?.Invoke();
            });

            // LayerIcon
            _layerIcon = this.Q<VisualElement>("Icon");

            // IconsModules
            _iconsModules = this.Q<VisualElement>("IconsModules");

            // Show/Hide button
            _showButton = this.Q<Button>("ShowButton");
            _showButton.clicked += () => { ShowLayer(true); };
            _hideButton = this.Q<Button>("HideButton");
            _hideButton.clicked += () => ShowLayer(false);
        }

        private void SetName(string name)
        {
            _layerName.value = name;
        }

        private void SetIcon(string guid)
        {
            VectorImage icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(guid);
            _layerIcon.style.backgroundImage = new StyleBackground(icon);
        }

        public void SetInfo(LBSLayer layer)
        {
            _target = layer;

            SetIcon(layer.iconGuid);
            SetName(layer.Name);

            layer.OnAddModule += (layer, module) =>
            {
                ShowModulesIcons();
            };

            ShowLayer(layer.IsVisible);
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
            _showButton.style.display = (!value) ? DisplayStyle.Flex : DisplayStyle.None;
            _hideButton.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            _target.IsVisible = value;
            _onVisibilityChange?.Invoke();
        }
    }
}