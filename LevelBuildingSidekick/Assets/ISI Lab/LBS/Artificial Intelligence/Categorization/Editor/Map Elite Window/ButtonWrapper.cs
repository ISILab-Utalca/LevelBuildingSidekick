using ISILab.Commons;
using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class ButtonWrapper : Button
    {

        object data;
        public string Text;

        public object Data { 
            get => data;
            set => data = value;
        }

        public ButtonWrapper()
        {
            var styleSheet = DirectoryTools.GetAssetByName<StyleSheet>("MapEliteUSS");
            this.styleSheets.Add(styleSheet);
            text = "0";
        }

        public ButtonWrapper(object data, Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
            var styleSheet = DirectoryTools.GetAssetByName<StyleSheet>("MapEliteUSS");
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

        internal void SetTexture(object value)
        {
            
            throw new NotImplementedException();
        }
    }
}