using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class ButtonWrapper : Button
    {
        public new class UxmlFactory : UxmlFactory<ButtonWrapper, VisualElement.UxmlTraits> { }

        object data;
        Label label;
        public string Text;
        public object Data { 
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public VisualElement Icon;

        public ButtonWrapper()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteElementUXML");
            visualTree.CloneTree(this);
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            style.backgroundColor = new Color(0,0,0,0);
            Icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Fitness");
            label.text = "0";
            Icon.style.backgroundImage = default;
        }

        public ButtonWrapper(object data, Vector2 size)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteElementUXML");
            visualTree.CloneTree(this);
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            Icon = this.Q<VisualElement>("Icon");
            label = this.Q<Label>("Fitness");
            label.text = "0";
            style.width = size.x;
            style.height = size.y;

            Data = data;
        }

        public void Init(Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
        }

        public Texture2D GetTexture()
        {

            if (data is IDrawable)
            {
                return (data as IDrawable).ToTexture();
            }
            return default;
        }

        internal void SetTexture(Texture2D texture)
        {
            style.backgroundImage = texture;
        }

        internal void UpdateLabel()
        {
            label.text = Text;
        }
    }
}