using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

namespace LBS.VisualElements
{
    public class SimplePallete : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<SimplePallete, VisualElement.UxmlTraits> { }
        #endregion

        private OptionView[] optionViews;

        // View
        private VisualElement content;
        private DropdownField dropdownGroup;
        private VisualElement icon;
        private Label nameLabel;
        private Button noElement;
        private Button addButton;

        // Events
        public Action<ChangeEvent<string>> OnChangeGroup;
        public Action<object> OnSelectOption;
        public Action OnAddOption;
        // public Action OnRemoveOption;    // Implementar (!)

        public bool ShowGroups
        {
            set
            {
                this.dropdownGroup.SetDisplay(value);
            }
        }

        public SimplePallete()
        {
            var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("SimplePallete");
            visualTree.CloneTree(this);

            // Content
            this.content = this.Q<VisualElement>("Content");

            // Change Group
            this.dropdownGroup = this.Q<DropdownField>("DropdownGroup");
            dropdownGroup.RegisterCallback<ChangeEvent<string>>(evt => OnChangeGroup?.Invoke(evt));

            // NameLabel
            this.nameLabel = this.Q<Label>("NameLabel");

            // AddButton
            this.addButton = this.Q<Button>("AddButton");
            addButton.clicked += () => OnAddOption?.Invoke();

            // NoElement
            this.noElement = this.Q<Button>("NoElement");

            // Icon
            this.icon = this.Q<VisualElement>("IconPallete");
            
        }

        public void SetOptions(object[] options, Action<OptionView, object> onSetView)
        {
            content.Clear();

            if (options.Length > 0)
            {
                //noElement.SetDisplay(false);

                this.optionViews = new OptionView[options.Length];

                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    var view = new OptionView(option, OnSelectOption, onSetView);
                    optionViews[i] = view;
                    content.Add(view);
                }
            }
            else
            {
                content.Add(noElement);
                //noElement.SetDisplay(true);
            }
        }

        public void SetIcon(Texture2D icon, Color color)
        {
            this.icon.style.backgroundImage = icon;
            this.icon.style.unityBackgroundImageTintColor = color;
        }

        public void SetName(string name)
        {
            this.nameLabel.text = name;
        }

        public void Repaint()
        {
            //throw new NotImplementedException();
        }
    }

}