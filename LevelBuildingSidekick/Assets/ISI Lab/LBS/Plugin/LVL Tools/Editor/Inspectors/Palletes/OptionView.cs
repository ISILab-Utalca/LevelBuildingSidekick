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
        private Label label;
        private Button button;

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
            set => button.style.backgroundImage = value;
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
            this.button = this.Q<Button>();
            button.clicked += () => { this.OnSelect?.Invoke(target); };

            // Init Fields
            this.target = target;

            // Init Events
            this.OnSelect = onSelect;

            this.OnSetView = onSetView;
            OnSetView?.Invoke(this, target);
        }
    }
}