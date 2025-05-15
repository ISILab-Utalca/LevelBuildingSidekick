using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    [UxmlElement]
    public partial class SimplePallete : VisualElement
    {
        #region FACTORY
        //public new class UxmlFactory : UxmlFactory<SimplePallete, VisualElement.UxmlTraits> { }
        #endregion

        private OptionView[] optionViews;
        private object[] options;
        private object selected;
        private object collectionSelected;
        private Action<OptionView, object> onSetView;

        #region FIELS VIEW
        private VisualElement content;
        private DropdownField dropdownGroup;
        private VisualElement icon;
        private Label nameLabel;
        private Button noElement;
        private Button addButton;
        private Button removeButton;
        
        private bool displayAddElement = true;
        #endregion

        #region EVENTS
        public event Action<ChangeEvent<string>> OnChangeGroup;
        public event Action<object> OnSelectOption;
        public event Action<object> OnRemoveOption;
        public event Action OnAddOption;
        public event Action OnRepaint;
        public event Func<object,string> OnSetTooltip;
        #endregion

        #region PROPERTIES
        public bool DisplayAddElement       
        {
            set => displayAddElement = value;
        }
        public object Selected
        {
            get => selected;
            set => selected = value;
        }

        public object CollectionSelected
        {
            get => collectionSelected;
            set => collectionSelected = value;
        }
        
        public object[] Options
        {
            get => options;
            set => options = value;
        }

        public bool ShowGroups
        {
            set => dropdownGroup.SetDisplay(value);
        }

        public bool ShowRemoveButton
        {
            set => removeButton.SetDisplay(value);
        }

        public bool ShowAddButton
        {
            set => addButton.SetDisplay(value);
        }
        
        public bool ShowNoElement
        {
            set => noElement.SetDisplay(value);
        }
        
        public bool ShowDropdown
        {
            set => dropdownGroup.SetDisplay(value);
        }
        
        #endregion


        #region CONSTRUCTORS
        public SimplePallete()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("SimplePallete");
            visualTree.CloneTree(this);

            // Content
            content = this.Q<VisualElement>("Content");
            content.style.flexDirection = FlexDirection.Row;         // Horizontal layout
            content.style.justifyContent = Justify.Center;     // Space items evenly
            content.style.alignItems = Align.Center;                 // Vertically center them
            
            // Change Group
            dropdownGroup = this.Q<DropdownField>("DropdownGroup");
            dropdownGroup.RegisterCallback<ChangeEvent<string>>(evt => OnChangeGroup?.Invoke(evt));

            // NameLabel
            nameLabel = this.Q<Label>("NameLabel");

            // AddButton
            addButton = this.Q<Button>("AddButton");
            addButton.clicked += () => OnAddOption?.Invoke();

            // removeButton
            removeButton = this.Q<Button>("DeleteButton");
            removeButton.clicked += () => OnRemoveOption?.Invoke(selected);

            // NoElement
            noElement = this.Q<Button>("NoElement");

            // Icon
            icon = this.Q<VisualElement>("IconPallete");

        }
        #endregion

        #region METHODS
        private void OnInternalSelectOption(object obj)
        {

            foreach (var optV in optionViews)
            {
                optV.SetSelected(false);
            }
            selected = obj;
            OnSelectOption?.Invoke(obj);

        }

        private void OnInternalRemoveOption(object obj)
        {
            foreach (var optV in optionViews)
            {
                optV.SetSelected(false);
            }
            selected = obj;
            OnRemoveOption?.Invoke(obj);
        }

        public void SetOptions(object[] options, Action<OptionView, object> onSetView)
        {
            this.options = options;
            this.onSetView = onSetView;
        }
        
        public void SetIcon(VectorImage icon, Color color)
        {
            this.icon.style.backgroundImage = new StyleBackground(icon);
            this.icon.style.unityBackgroundImageTintColor = color;
        }
        
        public void SetName(string name)
        {
            nameLabel.text = name;
        }

        public void DisplayContent(bool show)
        {
            if (show) content.style.display = DisplayStyle.Flex;
            else content.style.display = DisplayStyle.None;
        }
        
        public void Repaint()
        {
            OnRepaint?.Invoke();

            content.Clear();

            if (options != null && options.Length > 0)
            {
                optionViews = new OptionView[options.Length];

                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    var view = new OptionView(option, OnInternalSelectOption, OnRemoveOption, onSetView);
                    view.tooltip = OnSetTooltip?.Invoke(option);
                    optionViews[i] = view;
                    content.Add(view);
                }
            }
            else
            {
                if (displayAddElement)
                {
                  content.Add(noElement);
                }
            }

            if(selected != null)
            {
                var ov = optionViews?.ToList().Find(o 
                    => o != null && o.target != null && selected != null && o.target.Equals(selected));

                if (ov != null)  ov.SetSelected(true);
            }
        }
        #endregion
    }

}