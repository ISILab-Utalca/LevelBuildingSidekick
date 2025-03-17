using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Commons;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Generators;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class PopulationAssistantButtonResult : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        // display 2 decimals only
        private Label scoreLabel;
        // image displaying 3 dots and eventually the generated data
        private VisualElement image;
        
        #endregion

        #region FIELDS
        // result
        private object data;
        // value/score of the generated result
        private string value;
        
        #endregion

        #region EVENTS
        public Action OnExecute;
        #endregion

        #region PROPERTIES
        public object Data { 
            get => data;
            set => data = value;
        }
        #endregion

        #region CONSTRUCTORS
        public PopulationAssistantButtonResult()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantButtonResult");
            visualTree.CloneTree(this);
            
            scoreLabel = this.Q<Label>("ScoreValue");
            image = this.Q<Image>("Image");   
        }
        #endregion
        
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
            if (image == null) return;
            image.style.backgroundImage = texture;
        }

        internal void UpdateLabel(string score)
        {
            if (scoreLabel == null) return;
            scoreLabel.text = score;
        }
    }
}