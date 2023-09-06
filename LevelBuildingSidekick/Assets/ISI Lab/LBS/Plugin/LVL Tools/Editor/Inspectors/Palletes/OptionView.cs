using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

namespace LBS.VisualElements
{
    public class OptionView : VisualElement
    {
        private Color selected = new Color(1,1,1,0.1f);
        private Color nonSelected = new Color(1, 1, 1, 0f);

        private Label label;
        private Button button;
        private VisualElement icon;
        private VisualElement border;

        private object target;

        public Action<object> OnSelect;
        private Action<OptionView, object> OnSetView;

        #region PROPERTIES
        public object Target
        {
            get => target;
            set
            {
                target = value;
                OnSetView?.Invoke(this, target);
            }
        }

        public string Label
        {
            set => label.text = value;
        }

        public Texture2D Icon
        {
            set => icon.style.backgroundImage = value;
        }

        public Color Color
        {
            set => button.style.backgroundColor = value;
        }
        #endregion

        public OptionView(object target, Action<object> onSelect, Action<OptionView, object> onSetView)
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("OptionView");
            visualTree.CloneTree(this);

            // Init View
            this.label = this.Q<Label>();
            this.icon = this.Q<VisualElement>("Icon");
            this.border = this.Q<VisualElement>("Border");
            this.button = this.Q<Button>();
            button.clicked += () => { 
                this.OnSelect?.Invoke(target);
                SetSelected(true);
            };

            // Init Fields
            this.target = target;

            // Init Events
            this.OnSelect = onSelect;

            this.OnSetView = onSetView;
            OnSetView?.Invoke(this, target);
        }

        public void SetSelected(bool value)
        {
            if(value)
            {
                border.SetBorder(selected, 4);
                border.style.backgroundColor = selected;
            }
            else
            {
                border.SetBorder(nonSelected, 0);
                border.style.backgroundColor = nonSelected;
            }
        }
    }
}