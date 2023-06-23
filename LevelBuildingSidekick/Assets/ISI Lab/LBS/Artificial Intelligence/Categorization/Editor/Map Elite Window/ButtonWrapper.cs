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

        public ButtonWrapper()
        {
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            text = "0";
        }

        public ButtonWrapper(object data, Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            text = "0";

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
            text = Text;
        }
    }
}