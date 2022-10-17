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
        public object Data { 
            get
            {
                return data;
            }
            set
            {
                data = value;
                if(data is IDrawable)
                {
                    Icon.style.backgroundImage = (data as IDrawable).ToTexture();
                }
                else
                {
                    Icon.style.backgroundImage = default;
                }
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

            Icon.style.backgroundImage = default;
        }

        public ButtonWrapper(object data, Vector2 size)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("MapEliteElementUXML");
            visualTree.CloneTree(this);
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            Icon = this.Q<VisualElement>("Icon");

            style.width = size.x;
            style.height = size.y;

            Data = data;
        }

        public void Init(Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
        }
    }
}