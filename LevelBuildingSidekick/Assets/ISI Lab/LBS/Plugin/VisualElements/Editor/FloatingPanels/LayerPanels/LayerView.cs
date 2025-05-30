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

        private LBSLayer target;

        private TextField layerName;
        private VisualElement layerIcon;
        private VisualElement iconsModules;
        private Button showButton;
        private Button hideButton;

        public VisualElement _base;

        private Action onVisibilityChange;

        public event Action OnVisibilityChange
        {
            add => onVisibilityChange += value;
            remove => onVisibilityChange -= value;
        }

        public LayerView()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("LayerView");
            visualTree.CloneTree(this);

            this._base = this.Q<VisualElement>("Base");

            // LayerName
            this.layerName = this.Q<TextField>("Name");
            this.layerName.RegisterCallback<ChangeEvent<string>>(e =>
            {
                this.target.Name = e.newValue;
            });

            // LayerIcon
            this.layerIcon = this.Q<VisualElement>("Icon");

            // IconsModules
            this.iconsModules = this.Q<VisualElement>("IconsModules");

            // Show/Hide button
            this.showButton = this.Q<Button>("ShowButton");
            this.showButton.clicked += () => { ShowLayer(true); };
            this.hideButton = this.Q<Button>("HideButton");
            this.hideButton.clicked += () => ShowLayer(false);
        }

        private void SetName(string name)
        {
            layerName.value = name;
        }

        private void SetIcon(string guid)
        {
            VectorImage icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(guid);
            layerIcon.style.backgroundImage = new StyleBackground(icon);
        }

        public void SetInfo(LBSLayer layer)
        {
            this.target = layer;

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
            iconsModules.Clear();

            foreach (var module in target.Modules)
            {
                var icon = new VisualElement();
                icon.style.height = icon.style.width = 16;
                iconsModules.Add(icon);
            }
        }

        private void ShowLayer(bool value)
        {
            showButton.style.display = (!value) ? DisplayStyle.Flex : DisplayStyle.None;
            hideButton.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            target.IsVisible = value;
            onVisibilityChange?.Invoke();
        }
    }
}