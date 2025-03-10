using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement] 
    public partial class ClassFoldout : Foldout
    {
        // Define UXML attributes
        private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "Text", defaultValue = "ClassDropDown" };
        private readonly UxmlStringAttributeDescription m_IconPath = new UxmlStringAttributeDescription { name = "Icon-Path", defaultValue = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Logo.png" };

        // UI Elements
        public VisualElement icon;
        public ClassDropDown dropdown;
        public VisualElement content;

        object data = null;

        public new string text
        {
            get => dropdown?.label;
            set
            {
                if (dropdown != null)
                    dropdown.label = value;
            }
        }

        public object Data => data;
        public Action OnSelectChoice;
        
        public Type Type
        {
            get => dropdown.Type;
            set => dropdown.Type = value;
        }

        // new uxml constructor 
        public ClassFoldout()
        {

            // Initialize UI elements
            icon = new VisualElement() { name = "Icon" };
            dropdown = new ClassDropDown() { name = "ClassDropDown" };
            content = new VisualElement() { name = "Content" };

            content.style.flexGrow = 1;

            icon.style.width = icon.style.height = 12;
            icon.style.minHeight = icon.style.minWidth = 12;
            icon.style.marginRight = 6;

            dropdown.style.marginRight = 4;
            dropdown.style.flexShrink = 1;
            dropdown.style.flexGrow = 1;

            hierarchy.Add(icon);
            hierarchy.Add(dropdown);
            hierarchy.Add(content);
        }

        // previous create
        public void OnCreate(IUxmlAttributes bag, CreationContext cc)
        {
            // Apply attributes
            text = m_Label.GetValueFromBag(bag, cc);

            string path = m_IconPath.GetValueFromBag(bag, cc);
            Texture2D img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            icon.style.backgroundImage = img;

            if (img == null)
            {
                icon.style.display = DisplayStyle.None;
            }
        }
    }
}
